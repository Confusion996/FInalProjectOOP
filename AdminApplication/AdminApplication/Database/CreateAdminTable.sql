CREATE TABLE AdminAccounts (
    AdminId AUTOINCREMENT PRIMARY KEY,
    AdminName TEXT(100) NOT NULL,
    AdminEmail TEXT(100) NOT NULL,
    AdminPhone TEXT(20),
    AdminPassword TEXT(100) NOT NULL,
    Role TEXT(50) NOT NULL,
    LastLogin DATETIME,
    CreatedDate DATETIME DEFAULT Now(),
    IsActive YES/NO DEFAULT Yes
);

-- Insert default admin account
INSERT INTO AdminAccounts (AdminName, AdminEmail, AdminPhone, AdminPassword, Role, LastLogin)
VALUES (
    'Administrator',
    'admin@company.com',
    '1234567890',
    '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', -- 'admin' hashed with SHA-256
    'Super Admin',
    Now()
); 