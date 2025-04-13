## Overview

You've been provided with a deliberately vulnerable .NET Web API application. Your task is to approach this as a security professional, identify and document issues, and communicate your findings effectively.

## Challenge Tasks

### 1. Fix the Build Issue
- The Docker build for this application is broken
- Identify the issue and fix it so the application can be built and run successfully
- Document the problem and your solution

### 2. Security Vulnerability Assessment
- Conduct a comprehensive security assessment of the application
- Use appropriate security tools of your choice (e.g., OWASP ZAP, SonarQube, Snyk, etc.)
- Identify as many OWASP Top 10 vulnerabilities as possible

### 3. Documentation and Reporting
- Create a vulnerability report documenting your findings
- Include if possible:
  - Vulnerability descriptions
  - Severity ratings
  - Proof of concept/reproduction steps
  - Recommendations for remediation
- Write a professional email that would be sent to the development team explaining your findings
  - Should be technical but clear
  - Prioritize issues appropriately
  - Demonstrate good communication skills

### 4. Tool Infrastructure
- Provide Docker Compose file(s) for the security tools you used
- Optional: Create a GitHub Actions workflow for continuous security testing

## Submission Requirements

Please submit the following:
1. Fixed Dockerfile and any other code changes
2. Comprehensive security assessment report (PDF format preferred)
3. The email to the development team (as a separate file)
4. Docker Compose files and/or GitHub Actions workflow
5. Brief explanation of your approach and tools used

## Evaluation Criteria

You will be evaluated on:
- Technical approach and problem-solving ability (fixing the build)
- Thoroughness of security assessment
- Quality and clarity of documentation
- Communication skills demonstrated in the developer email

## Getting Started

### Prerequisites
- Docker
- .NET 6.0 SDK (for local development)

### Running Locally (Once Fixed)
```bash
cd BrokenWebAPI
dotnet run
```

### Expected Application Endpoints
- `GET /UserDevice` - Get random user-device pairs
- `POST /api/Auth/login` - Authenticate user
- `GET /api/Auth/admin/users` - Get all users (admin only)
- `POST /api/Auth/process-data` - Process user data
- `POST /api/File/upload` - Upload a file
- `GET /api/File/ping` - Ping a host
- `GET /api/File/get-file` - Get file contents

## Warning

**This application is deliberately vulnerable. DO NOT deploy it in a production environment or on a publicly accessible server.**

Good luck, and we look forward to reviewing your submission!
