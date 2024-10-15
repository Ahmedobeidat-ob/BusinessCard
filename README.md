# BusinessCard
# Business Card Management API (C# in .NET Core)

This is the backend API for managing business cards, built with C# and .NET Core. The API allows creating, viewing, deleting, exporting business cards, and filtering them based on various criteria.

## Features

### 1. Entity Definition
- **BusinessCard Model**: A business card entity is defined with all the required properties, such as:
  - Name
  - Date of Birth (DOB)
  - Gender
  - Email
  - Phone
  - Address  (optional)
  - Photo (optional)
  - CreatedAt (date when the card was created)

### 2. API Endpoints

#### 2.1 Create New Business Card
- **Endpoint**: `POST /api/businesscards`
- Accepts input from the frontend via the form or through file imports (XML, CSV, optional QR code).
- Validates the data and stores the business card in the database.

#### 2.2 View Business Cards
- **Endpoint**: `GET /api/businesscards`
- Retrieves a list of all stored business cards.

#### 2.3 Delete Business Card
- **Endpoint**: `DELETE /api/businesscards/{id}`
- Deletes a specific business card by its ID.

#### 2.4 Export Business Cards
- **Export to XML or CSV**:
  - **Endpoint (CSV)**: `GET /api/businesscards/export/csv`
  - **Endpoint (XML)**: `GET /api/businesscards/export/xml`
  - These endpoints allow users to export all business cards in XML or CSV format.
 
  - #### 2.5 Import Business Cards
  - **Import to XML or CSV**:
- **Endpoint (CSV)**: POST /api/businesscards/import/csv
- **Endpoint (XML)**: POST /api/businesscards/import/xml
These endpoints allow users to import business card data from XML or CSV files.

#### 2.6 Optional Filtering
- **Endpoint**: `GET /api/businesscards`
- Query parameters allow filtering by:
  - Name
  - Date of Birth (DOB)
  - Phone
  - Gender
  - Email

## Setup Instructions

### Prerequisites
- **.NET Core SDK** (version 7.0)
- **SQL Server**
