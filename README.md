[![Build Status](https://img.shields.io/github/actions/workflow/status/jjs98/cashbook/build.yaml?branch=main&style=for-the-badge)](https://github.com/jjs98/cashbook/actions/workflows/build.yaml?branch=main&style=for-the-badge)
[![Codecov](https://img.shields.io/codecov/c/github/jjs98/cashbook?style=for-the-badge)](https://codecov.io/gh/jjs98/cashbook)

# Cashbook
An open source app to manage your finances including bookings, fix costs, budgets and analysis.

## How to run the project

### Build the client

This project uses pnpm as the package manager.

- run `pnpm install` to install the dependencies.
- run `pnpm build` to build the client.
- run `pnpm start` to start the client.

### Build the server

The server uses .NET 10 so just open the Solution and run the project.

## Tests

### Client

The client uses Jest and playwright for testing.

- run `pnpm test` to run the tests.

### Server

The server uses TUnit for testing. Also TestContainers is used to run the tests in a real database.

## Shoutouts

- Thanks to [NGneers](https://github.com/NGneers) for [signal-translate](https://github.com/NGneers/signal-translate).
- Thanks to [MaSch0212](https://github.com/MaSch0212) for creating the [gOAst](https://github.com/MaSch0212/goast) tooling to create api code out of an openapi file.
