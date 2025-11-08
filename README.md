# BidOne Gateway API

## Overview

This project implements a **BidOne Gateway API** that exposes a stable API (v1) and orchestrates two upstream systems: 

1. **ERP Service** – Authoritative product data  
2. **Warehouse Service** – Product stock/availability  

The Gateway merges data from these services and provides a consistent, versioned API for clients, supporting evolution to v2 without breaking v1.

The project is implemented using **C# (.NET 8 Web API)**.

---

## Why C# (.NET 8)?

- Strong typing and compile-time safety for large integration scenarios.  
- Excellent support for asynchronous HTTP calls and middleware patterns.  
- Mature ecosystem for web APIs, caching, logging, and testing.  
- Built-in support for dependency injection, configuration, and idempotency patterns.  
- Familiar to enterprise-grade integration solutions and production-ready APIs.  

---

## Features

- **API Endpoints (v1)**
  - `GET /products` – Return merged ERP + Warehouse data.
  - `POST /products` – Create/update product in ERP with idempotency.
  - `GET /products/{id}` – Return merged product by ID.
  - `PUT /products/{id}` – Update product in ERP with idempotency.
  - `DELETE /products/{id}` – Delete product in ERP.

- **Idempotency**
  - Writes (POST/PUT) require an `Idempotency-Key` header.
  - Deduplication is performed by `key + operation + request body hash`.

- **Caching**
  - Thread-safe, in-memory cache for GET /products.
  - TTL to reduce upstream calls.

- **Versioning**
  - API versioning implemented for backward compatibility.
  - V2 support: optional response fields and/or new endpoints (e.g., `/v2/products`).

- **Configuration**
  - All sensitive data and configuration loaded from environment variables.
  - No secrets hardcoded in code.

- **Unit Tests**
  - xUnit is used for implementing unit tests.
  - Includes idempotency validation tests.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- IDE: Visual Studio 2022 or VS Code  
- Postman or similar tool for testing API endpoints  

---

## Getting Started

1. **Clone the repository**
   - https://github.com/AswinPerumal/Interview.BO
2. **Open BidOne.sln in visual studio**
3. **Set Presentation/BidOne.Gateway.API as startup project**
4. **Run using https**
5. **Application will launch using port 7216. https://localhost:7216/**
