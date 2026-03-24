from pydantic import BaseModel, Field
from typing import List, Optional
from .LocationDto import LocationDto
from .OrderEntreeItem import OrderEntreeItem
from .OrderSideItem import OrderSideItem
from .DrinkDto import DrinkDto


class BrowserOrder(BaseModel):
    isCardOrder: bool = False
    location: Optional[LocationDto] = None
    stationId: int = 0
    stationName: str = ""
    userName: str = ""
    entrees: List[OrderEntreeItem] = Field(default_factory=list)
    sides: List[OrderSideItem] = Field(default_factory=list)
    drinks: List[DrinkDto] = Field(default_factory=list)
