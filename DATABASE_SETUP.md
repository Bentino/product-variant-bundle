# Database Setup Guide

## Prerequisites

- Docker และ Docker Compose ติดตั้งแล้ว
- .NET 8 SDK ติดตั้งแล้ว
- Entity Framework Core Tools ติดตั้งแล้ว

> **หมายเหตุ**: คำสั่งในเอกสารนี้ใช้ `docker compose` (Docker Compose V2) หากระบบของคุณใช้เวอร์ชันเก่า ให้ใช้ `docker-compose` แทน

## Quick Setup

### 1. เริ่มต้น PostgreSQL ด้วย Docker

```bash
docker compose up -d postgres
```

### 2. รอให้ PostgreSQL พร้อมใช้งาน (ประมาณ 10-15 วินาที)

### 3. Apply Database Migrations

```bash
dotnet ef database update --project src/ProductVariantBundle.Infrastructure --startup-project src/ProductVariantBundle.Api
```

### หรือใช้ script อัตโนมัติ

```bash
./scripts/setup-database.sh
```

## Database Connection Details

- **Host**: localhost
- **Port**: 5432
- **Database**: ProductVariantBundle
- **Username**: postgres
- **Password**: postgres123

## Default Data

Database จะมี default warehouse ที่ชื่อ "Main Warehouse" (Code: MAIN) ถูกสร้างขึ้นอัตโนมัติ

## การจัดการ Database

### ดู status ของ migrations
```bash
dotnet ef migrations list --project src/ProductVariantBundle.Infrastructure --startup-project src/ProductVariantBundle.Api
```

### สร้าง migration ใหม่
```bash
dotnet ef migrations add <MigrationName> --project src/ProductVariantBundle.Infrastructure --startup-project src/ProductVariantBundle.Api
```

### ลบ database (ระวัง!)
```bash
dotnet ef database drop --project src/ProductVariantBundle.Infrastructure --startup-project src/ProductVariantBundle.Api
```

### หยุด PostgreSQL
```bash
docker compose down
```

## Troubleshooting

### ถ้า PostgreSQL ไม่สามารถเชื่อมต่อได้
1. ตรวจสอบว่า Docker กำลังทำงาน: `docker ps`
2. ตรวจสอบ logs: `docker compose logs postgres`
3. ลองรีสตาร์ท: `docker compose restart postgres`

### ถ้า Migration ล้มเหลว
1. ตรวจสอบว่า PostgreSQL พร้อมใช้งาน
2. ตรวจสอบ connection string ใน appsettings.json
3. ลองลบ database และสร้างใหม่: `dotnet ef database drop` แล้วตามด้วย `dotnet ef database update`