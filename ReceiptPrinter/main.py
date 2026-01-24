from fastapi import FastAPI, HTTPException
from DTOs.PrintOrderDto import PrintOrderDto
from utilities.format_order import format_order
from utilities.print_order import print_receipt

app = FastAPI()


@app.get("/")
def root():
    return {"Hello": "World"}


@app.post("/print-order")
def print_order(order: PrintOrderDto):
    try:
        formatted_lines = format_order(order)
        print_receipt(formatted_lines)
        return {"message": "Order printed successfully", "order_id": order.Id}
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error printing order: {str(e)}")
