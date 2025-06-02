!pip install --upgrade transformers
import pandas as pd
from datasets import Dataset
from transformers import BartTokenizer, BartForConditionalGeneration, TrainingArguments, Trainer


df = pd.read_csv('/content/BankFAQs.csv')
df = df[['Question', 'Answer']]


hf_dataset = Dataset.from_pandas(df)


model_name = 'facebook/bart-base'
tokenizer = BartTokenizer.from_pretrained(model_name)
model = BartForConditionalGeneration.from_pretrained(model_name)

# ✅ Preprocess function
def preprocess(batch):
    input_text = "question: " + batch['Question']
    target_text = batch['Answer']
    inputs = tokenizer(input_text, max_length=512, truncation=True, padding='max_length', return_tensors='pt')
    targets = tokenizer(target_text, max_length=128, truncation=True, padding='max_length', return_tensors='pt')
    return {
        'input_ids': inputs.input_ids.squeeze(),
        'attention_mask': inputs.attention_mask.squeeze(),
        'labels': targets.input_ids.squeeze()
    }


tokenized_dataset = hf_dataset.map(preprocess, remove_columns=['Question', 'Answer'])


training_args = TrainingArguments(
    output_dir='./bart-finetuned',
    num_train_epochs=3,
    per_device_train_batch_size=4,
    save_steps=100,
    save_total_limit=2,
    logging_dir='./logs',
    learning_rate=5e-5,
    weight_decay=0.01
)


trainer = Trainer(
    model=model,
    args=training_args,
    train_dataset=tokenized_dataset
)


trainer.train()

model.save_pretrained('/content/drive/MyDrive/bart-finetuned')
tokenizer.save_pretrained('/content/drive/MyDrive/bart-finetuned')

print("✅ Fine-tuning complete! Model saved at './bart-finetuned'")
