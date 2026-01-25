from pydantic import BaseModel
from typing import List
from datetime import datetime
from .FoodItemDto import FoodItemDto


class PrintOrderDto(BaseModel):
    id: int
    orderTime: datetime
    foodItems: List[FoodItemDto] = []
