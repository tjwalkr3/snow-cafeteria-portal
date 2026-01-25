from pydantic import BaseModel
from typing import Optional


class FoodItemOptionDto(BaseModel):
    id: int
    foodItemId: int
    foodOptionName: Optional[str] = None
