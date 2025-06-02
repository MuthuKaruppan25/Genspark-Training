from flask import Flask, request, jsonify
from transformers import BartTokenizer, BartForConditionalGeneration
from sentence_transformers import SentenceTransformer, util
import pandas as pd
import torch

app = Flask(__name__)

bart_model_path = './bart-finetuned'
bart_tokenizer = BartTokenizer.from_pretrained(bart_model_path)
bart_model = BartForConditionalGeneration.from_pretrained(bart_model_path)

def answer_with_bart(question):
    input_text = "question: " + question
    inputs = bart_tokenizer.encode(input_text, return_tensors='pt', max_length=512, truncation=True)

    outputs = bart_model.generate(
        inputs,
        max_length=128,
        num_beams=4,
        early_stopping=True
    )

    answer = bart_tokenizer.decode(outputs[0], skip_special_tokens=True)
    return answer

# === SENTENCE TRANSFORMER FAQ SETUP ===
faq_model = SentenceTransformer('all-MiniLM-L6-v2')
faq_df = pd.read_csv('./BankFAQs.csv')[['Question', 'Answer']]
faq_question_embeddings = faq_model.encode(faq_df['Question'].tolist(), convert_to_tensor=True)

def get_faq_best_answer(user_question, top_k=1):
    user_embedding = faq_model.encode(user_question, convert_to_tensor=True)
    cosine_scores = util.pytorch_cos_sim(user_embedding, faq_question_embeddings)[0]
    top_results = torch.topk(cosine_scores, k=top_k)

    results = []
    for score, idx in zip(top_results[0], top_results[1]):
        idx = idx.item()  # convert tensor to int here!
        results.append({
            'score': float(score),
            'question': faq_df['Question'][idx],
            'answer': faq_df['Answer'][idx]
        })
    return results

@app.route('/answer', methods=['POST'])
def bart_endpoint():
    data = request.json
    if not data or 'question' not in data:
        return jsonify({'error': 'Please provide a question field in the JSON payload.'}), 400

    question = data['question']

    # Check similarity first
    results = get_faq_best_answer(question, top_k=1)
    best_match = results[0]
    similarity_score = best_match['score']

    if similarity_score < 0.30:
        return jsonify({
            'question': question,
            'answer': "Sorry, I can't understand your question."
        })

    # Proceed to generate BART answer
    answer = answer_with_bart(question)
    return jsonify({'question': question, 'answer': answer, 'similarity_score': similarity_score})

@app.route('/faq', methods=['POST'])
def faq_endpoint():
    data = request.json
    if not data or 'question' not in data:
        return jsonify({'error': 'Please provide a question field in the JSON payload.'}), 400

    question = data['question']
    results = get_faq_best_answer(question, top_k=1)

    best_match = results[0]
    similarity_score = best_match['score']

    if similarity_score < 0.30:
        return jsonify({
            'question': question,
            'answer': "Sorry, I can't understand your question."
        })
    else:
        return jsonify({
            'question': best_match['question'],
            'answer': best_match['answer'],
            'similarity_score': similarity_score
        })

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)
