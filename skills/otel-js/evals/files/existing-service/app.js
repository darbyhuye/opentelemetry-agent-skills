const express = require('express');

const app = express();
app.get('/checkout/:id', (request, response) => {
  response.json({ id: request.params.id });
});
app.listen(8080);
