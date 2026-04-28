# STK-TRACK: Enterprise Warehouse Dashboard

A full-stack inventory management solution built with **Angular 17**, **ASP.NET Core 8**, and **SQLite**. Designed for high-performance stock tracking and professional audit reporting.

## 🚀 Technical Highlights
- **Full-Stack Integration**: Seamless communication between a decoupled Angular SPA and a RESTful .NET API.
- **Modern UI/UX**: Premium dashboard design using Angular Material, featuring real-time stat cards and pulsing low-stock alerts.
- **Professional Reporting**: Integrated **QuestPDF** engine for generating color-coded PDF inventory audits on the fly.
- **Optimized Persistence**: SQLite database with custom EF Core indexing for rapid search across thousands of SKUs.
- **Containerized**: Fully Dockerized for "one-click" local development and cloud deployment.

## 🛠️ Tech Stack
- **Frontend**: Angular 17, Material Design, RxJS, TypeScript.
- **Backend**: ASP.NET Core 8, Entity Framework Core.
- **Database**: SQLite (Optimized).
- **Reporting**: QuestPDF.
- **DevOps**: Docker, Vercel (Frontend), Render (Backend).

## 📦 Getting Started (Docker)
Run the entire stack with one command:
```bash
docker-compose up --build
```
The dashboard will be available at `http://localhost:4200`.

## 📈 Key Features
- **Smart Monitoring**: Automatic KPI calculation for total stock and reorder alerts.
- **Live Search**: Instant filtering with database-level optimization.
- **PDF Export**: Generate high-fidelity audit reports.
- **Responsive Layout**: Designed for both desktop warehouse stations and mobile inventory checks.
