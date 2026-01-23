from pydantic import BaseModel
from typing import Optional, List
from .FoodItemOptionDto import FoodItemOptionDto


class FoodItemOrderDto(BaseModel):
    Id: int
    OrderId: int
    StationId: int
    SaleCardId: Optional[int] = None
    SaleSwipeId: Optional[int] = None
    SwipeCost: Optional[int] = None
    CardCost: Optional[float] = None
    Special: bool
    Options: List[FoodItemOptionDto] = []
