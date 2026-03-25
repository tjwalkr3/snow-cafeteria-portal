from pydantic import BaseModel
from typing import Optional
from decimal import Decimal


class SideDto(BaseModel):
    id: int = 0
    stationId: int = 0
    sideName: str = ""
    sideDescription: Optional[str] = None
    sidePrice: Decimal = Decimal("0")
    imageUrl: Optional[str] = None
    inStock: bool = True
