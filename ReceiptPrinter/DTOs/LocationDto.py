from pydantic import BaseModel
from typing import Optional


class LocationDto(BaseModel):
    id: int = 0
    locationName: str = ""
    locationDescription: str = ""
    iconId: Optional[int] = None
    iconBootstrapName: Optional[str] = None
    printerUrl: Optional[str] = None
