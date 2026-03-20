from pydantic import BaseModel
from typing import Optional
from decimal import Decimal


class EntreeDto(BaseModel):
    id: int = 0
    stationId: int = 0
    entreeName: str = ""
    entreeDescription: Optional[str] = None
    entreePrice: Decimal = Decimal("0")
    imageUrl: Optional[str] = None
    inStock: bool = True
