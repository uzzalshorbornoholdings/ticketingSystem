CREATE DATABASE IF NOT EXISTS bitsward_tickets;
USE bitsward_tickets;

-- 1. Departments Table
CREATE TABLE IF NOT EXISTS departments (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) UNIQUE NOT NULL
) ENGINE=InnoDB;

-- 2. Employees Table
CREATE TABLE IF NOT EXISTS employees (
    id VARCHAR(50) PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    designation VARCHAR(150),
    department_id INT,
    reports_to_id VARCHAR(50),
    FOREIGN KEY (department_id) REFERENCES departments(id) ON DELETE SET NULL,
    FOREIGN KEY (reports_to_id) REFERENCES employees(id) ON DELETE SET NULL
) ENGINE=InnoDB;

-- 3. SLA Plans Table
CREATE TABLE IF NOT EXISTS slas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(50) UNIQUE NOT NULL,
    resolution_hours INT NOT NULL,
    alert_threshold_minutes INT NOT NULL
) ENGINE=InnoDB;

-- 4. Tickets Table
CREATE TABLE IF NOT EXISTS tickets (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    type VARCHAR(20) NOT NULL, -- 'INC', 'SR', 'CR'
    priority VARCHAR(10) NOT NULL, -- 'P1', 'P2', 'P3', 'P4'
    sla_id INT,
    status VARCHAR(50) NOT NULL DEFAULT 'Open',
    creator_employee_id VARCHAR(50),
    assigned_employee_id VARCHAR(50),
    locked_by VARCHAR(50),
    locked_until DATETIME,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (sla_id) REFERENCES slas(id) ON DELETE SET NULL,
    FOREIGN KEY (creator_employee_id) REFERENCES employees(id) ON DELETE SET NULL,
    FOREIGN KEY (assigned_employee_id) REFERENCES employees(id) ON DELETE SET NULL,
    FOREIGN KEY (locked_by) REFERENCES employees(id) ON DELETE SET NULL
) ENGINE=InnoDB;

-- 5. Ticket Threads Table
CREATE TABLE IF NOT EXISTS ticket_threads (
    id INT AUTO_INCREMENT PRIMARY KEY,
    ticket_id INT NOT NULL,
    employee_id VARCHAR(50) NOT NULL,
    message TEXT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ticket_id) REFERENCES tickets(id) ON DELETE CASCADE,
    FOREIGN KEY (employee_id) REFERENCES employees(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 6. Tasks Table
CREATE TABLE IF NOT EXISTS tasks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    ticket_id INT,
    title VARCHAR(255) NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'Pending',
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ticket_id) REFERENCES tickets(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 7. Change Requests Table
CREATE TABLE IF NOT EXISTS change_requests (
    id INT AUTO_INCREMENT PRIMARY KEY,
    ticket_id INT UNIQUE NOT NULL,
    risk_score VARCHAR(20) NOT NULL, -- 'Low', 'Medium', 'High'
    cab_approved TINYINT(1) DEFAULT 0,
    maintenance_window_start DATETIME NULL,
    maintenance_window_end DATETIME NULL,
    pir_status VARCHAR(50) DEFAULT 'Pending',
    pir_notes TEXT NULL,
    FOREIGN KEY (ticket_id) REFERENCES tickets(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 8. Audit Logs Table
CREATE TABLE IF NOT EXISTS audit_logs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    ticket_id INT NULL,
    employee_id VARCHAR(50) NOT NULL,
    action VARCHAR(100) NOT NULL,
    details TEXT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ticket_id) REFERENCES tickets(id) ON DELETE SET NULL,
    FOREIGN KEY (employee_id) REFERENCES employees(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- 9. Seed SLA Data
INSERT INTO slas (name, resolution_hours, alert_threshold_minutes) VALUES
('P1', 2, 30),
('P2', 8, 120),
('P3', 24, 360),
('P4', 72, 1440)
ON DUPLICATE KEY UPDATE 
resolution_hours = VALUES(resolution_hours), 
alert_threshold_minutes = VALUES(alert_threshold_minutes);

-- 10. Users Table (Role-based Authentication)
CREATE TABLE IF NOT EXISTS users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    employee_id VARCHAR(50) UNIQUE,
    username VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    salt VARCHAR(50) NOT NULL,
    role VARCHAR(50) NOT NULL,
    FOREIGN KEY (employee_id) REFERENCES employees(id) ON DELETE CASCADE
) ENGINE=InnoDB;

