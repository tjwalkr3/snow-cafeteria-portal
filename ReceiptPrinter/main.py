from fastapi import FastAPI
from DTOs.PrintOrderDto import PrintOrderDto

app = FastAPI()

@app.get("/")
def root():
  return {"Hello": "World"}

@app.post("/print-order")
def print_order(order: PrintOrderDto):
  return {"message": "Order received", "order_id": order.OrderId}
