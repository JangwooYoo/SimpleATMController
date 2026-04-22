# SimpleATMController

간단한 ATM 컨트롤러 예제입니다. UI는 포함하지 않고, 카드 입력부터 PIN 검증, 계좌 선택, 잔액 조회, 입금, 출금까지의 흐름을 코드로 구현했습니다.

## 프로젝트 구성

- `SimpleATMController`
  - ATM 도메인 모델
  - 외부 연동을 위한 인터페이스
  - 핵심 비즈니스 로직을 처리하는 `ATMController`
- `SimpleATMController.Tests`
  - 컨트롤러 단위 테스트
  - 은행 시스템, 카드 리더기, 현금 장치를 대체하는 테스트용 Mock 구현

## 주요 흐름

구현된 기본 흐름은 다음과 같습니다.

1. 카드 삽입
2. PIN 입력
3. 계좌 선택
4. 잔액 조회
5. 입금 또는 출금

## 설계 의도

실제 은행 시스템이나 ATM 하드웨어는 이번 범위에 포함하지 않았습니다. 대신 아래 항목들은 인터페이스로 분리했습니다.

- `IBankSystem`
- `ICardReader`
- `ICashBin`

이렇게 분리해 두면 이후 실제 은행 API나 하드웨어 연동 구현을 추가하더라도 `ATMController` 자체는 테스트 가능한 상태를 유지할 수 있습니다.

## 개발 환경

- .NET 10 SDK
- xUnit

## 저장소 복제

```powershell
git clone <repository-url>
cd SimpleATMController
```

## 빌드 방법

```powershell
dotnet build .\SimpleATMController\SimpleATMController.csproj
```

## 테스트 실행 방법

```powershell
dotnet test .\SimpleATMController.Tests\SimpleATMController.Tests.csproj
```

## 테스트 범위

테스트에는 아래 항목이 포함되어 있습니다.

- 카드 삽입
- 올바른 PIN / 잘못된 PIN 처리
- 인증 이후 계좌 목록 조회
- 계좌 선택
- 잔액 조회
- 입금 처리
- 출금 처리
- 잔액 부족 처리
- ATM 현금 부족 처리
- 세션 종료 처리
- 전체 사용자 흐름 검증

## 테스트 코드 위치

테스트 코드는 아래 경로에 포함되어 있습니다.

- `SimpleATMController.Tests/ATMControllerTests.cs`
- `SimpleATMController.Tests/Mocks/MockBankSystem.cs`
- `SimpleATMController.Tests/Mocks/MockCardReader.cs`
- `SimpleATMController.Tests/Mocks/MockCashBin.cs`

## 테스트 케이스 예시

- `InsertCard_ShouldSetCardInSession`
- `EnterPin_WithCorrectPin_ShouldAuthenticate`
- `EnterPin_WithIncorrectPin_ShouldThrowInvalidPinException`
- `GetAvailableAccounts_AfterAuthentication_ShouldReturnAccounts`
- `SelectAccount_WithValidAccount_ShouldSetSelectedAccount`
- `GetBalance_AfterSelectingAccount_ShouldReturnCorrectBalance`
- `Deposit_WithValidAmount_ShouldIncreaseBalance`
- `Withdraw_WithValidAmount_ShouldDecreaseBalance`
- `Withdraw_WithInsufficientFunds_ShouldThrowInsufficientFundsException`
- `Withdraw_WithInsufficientCash_ShouldThrowInsufficientCashException`
- `CompleteFlow_InsertCardToPinToAccountToBalance_ShouldWork`

## 참고

이 프로젝트는 콘솔 UI, 웹 API, DB 연동 없이 순수한 컨트롤러 로직과 테스트에 집중합니다. 다른 엔지니어가 이 코드를 기반으로 콘솔, 데스크톱, 웹, 모바일 UI를 연결할 수 있도록 구성했습니다.

## 제출 전 체크리스트

- 루트 `README.md` 포함
- 컨트롤러 구현 포함
- 테스트 코드 포함
- `dotnet build .\SimpleATMController\SimpleATMController.csproj` 성공
- `dotnet test .\SimpleATMController.Tests\SimpleATMController.Tests.csproj` 성공

---

# SimpleATMController

This project is a simple ATM controller example. It does not include any graphical or console UI. Instead, it focuses on implementing the core flow from card insertion to PIN verification, account selection, balance inquiry, deposit, and withdrawal.

## Project Structure

- `SimpleATMController`
  - ATM domain models
  - Interfaces for external integrations
  - `ATMController` containing the core business logic
- `SimpleATMController.Tests`
  - Unit tests for the controller
  - Mock implementations for the bank system, card reader, and cash hardware

## Main Flow

The following flow is implemented:

1. Insert card
2. Enter PIN
3. Select account
4. Check balance
5. Deposit or withdraw

## Design Intent

This project does not implement a real bank system or ATM hardware. Instead, the following interfaces are used to separate the controller from external dependencies:

- `IBankSystem`
- `ICardReader`
- `ICashBin`

This makes it possible to keep the `ATMController` testable even when real integrations are added later.

## Development Environment

- .NET 10 SDK
- xUnit

## Clone the Repository

```powershell
git clone <repository-url>
cd SimpleATMController
```

## Build

```powershell
dotnet build .\SimpleATMController\SimpleATMController.csproj
```

## Run Tests

```powershell
dotnet test .\SimpleATMController.Tests\SimpleATMController.Tests.csproj
```

## Test Coverage

The test suite includes the following scenarios:

- Card insertion
- Valid PIN / invalid PIN handling
- Retrieving available accounts after authentication
- Account selection
- Balance inquiry
- Deposit
- Withdrawal
- Insufficient account balance handling
- Insufficient ATM cash handling
- Session termination
- End-to-end flow validation

## Test Code Location

The test code is included in the following files:

- `SimpleATMController.Tests/ATMControllerTests.cs`
- `SimpleATMController.Tests/Mocks/MockBankSystem.cs`
- `SimpleATMController.Tests/Mocks/MockCardReader.cs`
- `SimpleATMController.Tests/Mocks/MockCashBin.cs`

## Example Test Cases

- `InsertCard_ShouldSetCardInSession`
- `EnterPin_WithCorrectPin_ShouldAuthenticate`
- `EnterPin_WithIncorrectPin_ShouldThrowInvalidPinException`
- `GetAvailableAccounts_AfterAuthentication_ShouldReturnAccounts`
- `SelectAccount_WithValidAccount_ShouldSetSelectedAccount`
- `GetBalance_AfterSelectingAccount_ShouldReturnCorrectBalance`
- `Deposit_WithValidAmount_ShouldIncreaseBalance`
- `Withdraw_WithValidAmount_ShouldDecreaseBalance`
- `Withdraw_WithInsufficientFunds_ShouldThrowInsufficientFundsException`
- `Withdraw_WithInsufficientCash_ShouldThrowInsufficientCashException`
- `CompleteFlow_InsertCardToPinToAccountToBalance_ShouldWork`

## Notes

This project focuses on the controller logic and unit tests only. It does not include a console UI, web API, or database integration. The code is structured so that another engineer can build a console, desktop, web, or mobile UI on top of it.

## Submission Checklist

- Root `README.md` included
- Controller implementation included
- Test code included
- `dotnet build .\SimpleATMController\SimpleATMController.csproj` succeeds
- `dotnet test .\SimpleATMController.Tests\SimpleATMController.Tests.csproj` succeeds
