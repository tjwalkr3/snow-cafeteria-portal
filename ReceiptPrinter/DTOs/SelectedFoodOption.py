from pydantic import BaseModel
from .FoodOptionDto import FoodOptionDto
from .FoodOptionTypeDto import FoodOptionTypeDto


class SelectedFoodOption(BaseModel):
    option: FoodOptionDto = FoodOptionDto()
    optionType: FoodOptionTypeDto = FoodOptionTypeDto()
