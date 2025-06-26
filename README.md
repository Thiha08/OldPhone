# OldPhone Keypad System

A .NET 9.0 console application that simulates the classic mobile phone keypad input system. 
I built this as a coding challenge to simulate the classic mobile phone keypad input system.

## What It Does

- **Classic Keypad Feel**: Press '2' once for 'A', twice for 'B', three times for 'C'
- **Interactive Console**: Type keys and see the text appear in real-time
- **Batch Processing**: Test entire strings like "4433555 555666#" to get "HELLO"
- **Timer Logic**: Wait 1 second and the character cycles automatically
- **Backspace Support**: Press '*' to delete the last character

## Quick Start

```bash
# Clone and run
git clone <your-repo>
cd OldPhone
dotnet run --project OldPhone.ConsoleApp
```

## How to Use

The app gives you a simple menu:

```
*** Available Commands ***
1-9/0: Input keys
*: Backspace
#: End input
O: OldPhonePad(string input)
Q: Quit
**************************
```

### Keypad Mapping
```
1: &'(    2: ABC    3: DEF
4: GHI    5: JKL    6: MNO
7: PQRS   8: TUV    9: WXYZ
0: Space
```

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

## Project Structure

```
OldPhone/
├── OldPhone.ConsoleApp/     # Main app
│   ├── Services/            # Business logic
│   ├── KeyMap.cs            # Key mappings
│   ├── OldPhoneApp.cs       # UI logic
│   └── Program.cs           # Entry point
├── OldPhone.Tests/          # Tests
└── OldPhone.sln             # Solution
```

## What I Built

- **Dependency injection** - Clean, testable architecture
- **Event-driven updates** - Real-time text changes
- **Timer management** - Proper resource disposal
- **Docker containerization** - Production-ready deployment

**Code Quality:**
- 57% test coverage (9 tests)
- Null-safe operations
- Proper error handling
- Clean separation of concerns

## Running Tests

```bash
dotnet test
```

The tests cover the main functionality and lifecycle management. I focused on testing the core logic rather than the UI interactions.

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