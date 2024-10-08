# Load model directly
from transformers import AutoModelForSeq2SeqLM, T5Tokenizer
from flask import Flask, request, jsonify

# Initialize Flask
app = Flask(__name__)


# Load the tokenizer and model
model_name = "gaussalgo/T5-LM-Large-text2sql-spider"
tokenizer = T5Tokenizer.from_pretrained(model_name)
model = AutoModelForSeq2SeqLM.from_pretrained(model_name)
#tokenizer.save_pretrained(f"cache3/tokenizer/{model_name}")
#model.save_pretrained(f"cache3/model/{model_name}")
 
def translate_to_sql_select(english_query):
  question = english_query
  schema="Sales(SalesOrderID:int, OrderDate:datetime, DueDate:datetime,OrderStatus:int,TotalDue:money, CustomerID:int,FirstName:name, LastName:name, Title: nvarchar, SalesOrderDetailID:int, OrderQty:int, LineTotal:int,ProductID:int, ProductName:name,ProductNumber:nvarchar, StandardCost:money,ListPrice:money)"
  
  #print(schema)

  input_text = " ".join(["Question: ",question, "Schema:", schema])

  
  model_inputs = tokenizer(input_text, return_tensors="pt")
  outputs = model.generate(**model_inputs, max_length=512)
 
  sql_query = tokenizer.decode(outputs[0], skip_special_tokens=True)
  return sql_query

@app.route('/generate_sql', methods=['POST'])
def generate_sql():
    # Get the natural language query from the request
    data = request.json
    natural_query = data.get("query", "")
    print("input:", natural_query)
    # Tokenize the input text
    english_query = natural_query
    sql_query = translate_to_sql_select(english_query)
    print("SQL Query:", sql_query)
    # Return the SQL query as a JSON response
    return jsonify({"sql": sql_query})

if __name__ == "__main__":
    app.run(port=5000)