from fastapi import FastAPI, HTTPException, Request
from DTOs.BrowserOrder import BrowserOrder
from utilities.format_order import format_order
from utilities.print_order import print_receipt

app = FastAPI()


@app.get("/")
def root():
    return {"Hello": "World"}


@app.post("/print-order")
async def print_order(request: Request):
    body = await request.body()
    print(f"Received JSON: {body.decode('utf-8')}")

    try:
        order = BrowserOrder.model_validate_json(body)
        formatted_lines = format_order(order)
        print_receipt(formatted_lines)
        return {"message": "Order printed successfully", "user_name": order.userName}
    except Exception as e:
        print(f"Error: {str(e)}")
        raise HTTPException(status_code=500, detail=f"Error printing order: {str(e)}")
