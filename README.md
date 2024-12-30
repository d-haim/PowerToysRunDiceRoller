# PowerToys Run Dice Roller Plugin

This plugin allows rolling dice using the Power Toys Run Launcher.

## Installation

0. [Install PowerToys](https://docs.microsoft.com/en-us/windows/powertoys/install)
1. Exit PowerToys
2. Download the `.zip` file applicable for your platform for the releases page.
3. Extract it to:
   - `%LocalAppData%\Microsoft\PowerToys\PowerToys Run\Plugins`
4. Start PowerToys

## Usage
Use action `rl` to start to plugin, followed by common dice notation.
>For example: `rl 2d6` will create a roll of two six sided dice.

You can roll multiple sets of dice by separating them with spaces.
>For example: `rl 4d20 1d8 2d10` will create a roll combined with all of these dice sets.
The output will be the result for each separate roll with a total sum.

You can also add modifier (+|-) to the roll, by adding the modifier to the end of the notation.
>For example: `rl 3d4+2` will a roll three, four sided dice and will add a 2 to the final result of the roll.
