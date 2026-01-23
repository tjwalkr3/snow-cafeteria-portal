from pydantic import BaseModel
from typing import List
from datetime import datetime
from .FoodItemOrderDto import FoodItemOrderDto


class PrintOrderDto(BaseModel):
    OrderId: int
    OrderTime: datetime
    TotalPrice: float
    FoodItems: List[FoodItemOrderDto] = []
