from pydantic import BaseModel
from typing import List, Optional
from datetime import datetime
from .LocationDto import LocationDto
from .OrderEntreeItem import OrderEntreeItem
from .OrderSideItem import OrderSideItem
from .DrinkDto import DrinkDto


class PrintOrderDto(BaseModel):
    id: int
    orderTime: datetime
    isCardOrder: bool = False
    location: Optional[LocationDto] = None
    stationId: int = 0
    stationName: str = ""
    entrees: List[OrderEntreeItem] = []
    sides: List[OrderSideItem] = []
    drinks: List[DrinkDto] = []
