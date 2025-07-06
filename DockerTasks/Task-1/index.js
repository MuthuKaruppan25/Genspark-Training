const express = require('express');
const cors = require('cors');   // import cors
const app = express();
const port = 8000;

// enable CORS for all origins
app.use(cors());

app.get('/', (req, res) => {
  res.send('Hello World');
});

app.listen(port, () => {
  console.log(`Server running on port ${port}`);
});
