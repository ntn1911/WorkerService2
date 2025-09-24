Windows Service: Scan Folder, Upload Images to Supabase Storage, and Save Link to SQL Server (.NET Core)
Overview

This project provides a robust Windows Service built with .NET Core. It automatically scans a local folder for new image files, uploads them to Supabase Storage, and saves the public URL of each image to a SQL Server database. It’s ideal for automating media synchronization, cloud backups, or building an image upload pipeline for your system.

Main Features

Automatic Folder Scanning:
Monitors a designated folder and detects new image files (JPG, PNG, etc.).

Upload to Supabase Storage:
Images are uploaded to a Supabase Storage bucket, using a structured path (e.g., by date and unique name) to prevent naming conflicts.

Public URL Generation:
Automatically generates a public URL for each uploaded file, allowing external access.

Save to SQL Server:
Image metadata and the public URL are saved into a SQL Server database table for easy management and integration.

Scheduled Jobs:
Uses Quartz.NET for flexible scheduling—run jobs at fixed times or intervals (e.g., every 5 minutes, or at 5:30 PM daily).

Status Management:
After processing, files are moved to a "Processed" or "Failed" folder, depending on the result.

Use Cases

Automatically sync images from local systems to the cloud.

Media workflow for product images, scanned documents, or kiosk uploads.

Reliable, unattended background uploads for businesses or creative projects.

How to Use

Setup:

Create a Supabase project and storage bucket, and obtain your API Key.

Prepare a SQL Server database and create a table for storing image metadata and URLs.

Build & Deploy:

Use dotnet publish to build the application.

Install the service using Windows Service commands (sc create).

Run:

The service runs in the background, processing files automatically according to the schedule.

Processing logs and status are available for easy monitoring and troubleshooting.

Technologies Used

.NET Core Worker Service

Supabase Storage SDK (C#)

Quartz.NET (Job scheduling)

SQL Server (Dapper or EF Core)

Windows Services (background automation)

Notes

The source code can be extended for more advanced features, such as image resizing, notifications, or supporting other cloud storage providers.

The service can be adapted to other environments (Linux, Docker, etc.) with minor changes.

This repository is ideal for backend developers, DevOps engineers, or businesses looking to automate and modernize their media handling and cloud uploads using Supabase.
