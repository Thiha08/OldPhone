# OldPhone Keypad System

A .NET 9.0 console application that simulates the classic mobile phone keypad input system. 
I built this as a coding challenge to simulate the classic mobile phone keypad input system.

## What It Does

- **Classic Keypad Feel**   : Press '2' once for 'A', twice for 'B', three times for 'C'
- **Interactive Console**   : Type keys and see the text appear in real-time
- **Batch Processing**      : Test entire strings like "4433555 555666#" to get "HELLO"
- **Timer Logic**           : Wait 1 second and the character cycles automatically
- **Backspace Support**     : Press '*' to delete the last character
- **Smart Input Validation**: Only accepts valid phone keypad characters
- **Dual Input Modes**      : Switch between single-key and string input modes
- **Real-time Events**      : Watch text change as you type

## Quick Start

```bash
# Clone and run
git clone https://github.com/Thiha08/OldPhone.git
cd OldPhone
dotnet run --project OldPhone.ConsoleApp
```

## How to Use

The app gives you a simple menu with two input modes:

```
*** Available Commands ***
O: OldPhonePad(string input) - Default Mode
S: Switch to Single Key Input Mode
M: Display Commands
Q: Quit

1-9: Input keys
0: Space
*: Backspace
#: End input

**************************
```

### Keypad Mapping
```
1: &'(    2: ABC    3: DEF
4: GHI    5: JKL    6: MNO
7: PQRS   8: TUV    9: WXYZ
0: Space
```

### Input Modes

#### String Input Mode (Default)
- Enter entire strings at once
- Perfect for testing and batch processing
- Invalid characters are automatically filtered out
- Example: `"4433555 555666#"` → `"HELLO"`

#### Single Key Input Mode
- Press one key at a time
- See real-time text updates
- Invalid keys are ignored (no beep, just silent)
- Great for interactive typing experience

### Examples

#### Interactive Input
1. Press `2` → "A"
2. Press `2` again → "B" (cycles through ABC)
3. Press `*` → Backspace
4. Press `#` → End input

#### Batch Processing (OldPhonePad)
Use command `O` to process entire strings:
```
OldPhonePad("222 2 22") => "CAB"
OldPhonePad("33#") => "E"
OldPhonePad("227*#") => "B"
OldPhonePad("4433555 555666#") => "HELLO"
OldPhonePad("8 88777444666*664#") => "TURING"
```

#### Mode Switching
- Type `S` to switch to single key mode
- Type `O` to switch back to string mode
- Type `M` to display commands again
- Type `Q` to quit

## Project Structure

```
OldPhone/
├── OldPhone.ConsoleApp/                # Main application
│   ├── Services/                       # Business logic & interfaces
│   │   ├── IOldPhoneKeyService.cs
│   │   └── OldPhoneKeyService.cs
│   ├── PhoneCmdHelper.cs               # Input validation & helper
│   ├── KeyMap.cs                       # Key mappings
│   ├── Constants.cs                    # Configuration constants
│   ├── OldPhoneApp.cs                  # UI logic & mode management
│   └── Program.cs                      # Entry point
├── OldPhone.Tests/                     # Comprehensive test suite
│   └── OldPhoneKeyServiceTests.cs
└── OldPhone.sln                        # Solution file
```

## What I Built

### Architecture & Design
- **Dependency Injection**  : Clean, testable architecture with interfaces
- **Event-Driven Updates**  : Real-time `TextChanged` and `TextCompleted` events
- **Helper Classes**        : Reusable `PhoneCmdHelper` for input validation
- **Regex Validation**      : Smart character filtering with `^[1-90*#MOQS\s]+$`

### Code Quality
- **Comprehensive Testing** : 15+ unit tests covering main scenarios in core services
- **Event Testing**         : Both `TextChanged` and `TextCompleted` events tested
- **Input Validation**      : Prevents invalid characters from being entered
- **Resource Management**   : Proper timer disposal and memory management

### User Experience
- **Smart Input Filtering** : Only valid characters accepted
- **Real-time Feedback**    : See text changes as you type
- **Mode Switching**        : Seamless transition between input modes
- **Clear Commands**        : Easy-to-understand interface

## Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage (if available)
dotnet test --collect:"XPlat Code Coverage"
```

The tests cover the core logic but not the UI interactions
I focused on testing the core logic.

## Docker (Optional)

If you want to run it in a container:

```bash
docker build -t oldphone-app ./OldPhone.ConsoleApp
docker run -it oldphone-app
```

I used Alpine Linux for a smaller, more secure image.

## Thank You

Thank you for reviewing this submission.  
I truly enjoyed the challenge and hope this solution meets your expectations — both technically and in spirit.

— Thiha Kyaw Htin