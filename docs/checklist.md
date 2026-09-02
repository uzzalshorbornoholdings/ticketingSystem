# Bitsward ITSM & Issue Ticketing System — Feature Checklist

This document tracks all features, modules, architectural layers, and roadmap capabilities for the **Bitsward ITSM Desktop Application**, cross-referencing the master requirements (`idea.txt` and `BaselinePrompt.txt`).

---

## 📊 Summary Dashboard

| Functional Phase | Status | Implemented Components |
| :--- | :---: | :--- |
| **Phase 1: Data Foundation & CSV Sync** | ✅ Complete | MySQL Schema, OrganogramSync, Dual-Pass Tree Ingestion, Auto-DB Init |
| **Phase 2: Triage & SLA Engines** | ✅ Complete | Keyword Classifier, 3-Tier Assignment Router, Business-Hours SLA Engine |
| **Phase 3: Modern UI Implementation** | ✅ Complete | Dark-themed LoginForm, MainForm Tabbed Queues, Dynamic Detail View, AdminForm |
| **Phase 4: Lifecycle & CR Subsystems** | ✅ Complete | Soft Lock, Thread Comments, Sub-Task Splitting, CAB Review, Risk Profiler, PIR Engine |
| **Phase 5: Alerts, Reporting & Admin** | ✅ Complete | Toast Popups, Background Checker, AuditLogForm, TasksForm, Admin Console |
| **Future Extensions / Extended Roadmap** | ⏳ Planned | Attachment Uploads, Web Customer Portal, SMTP Auto-Responder, PDF Export |

---

## 🚀 Detailed Phase-by-Phase Checklist

### ✅ PHASE 1: Data Foundation & CSV Sync
- [x] **MySQL Schema Definition (`docs/schema.sql`)**:
  - [x] `departments`: Department entity registry with unique constraint.
  - [x] `employees`: Profile metadata, designation, department FK, and hierarchical self-referencing `reports_to_id` FK.
  - [x] `slas`: SLA category specifications (P1: 2h/30m, P2: 8h/2h, P3: 24h/6h, P4: 72h/24h).
  - [x] `tickets`: Core ticket tracking table with type (INC, SR, CR), priority (P1–P4), status, creator, assignee, soft lock timestamps, and SLA linking.
  - [x] `ticket_threads`: Conversational audit log and comments history per ticket.
  - [x] `tasks`: Sub-tasks linked to parent tickets with statuses (`Pending`, `In Progress`, `Done`, `Cancelled`).
  - [x] `change_requests`: Extended CR metadata including `risk_score`, `cab_approved`, maintenance window start/end, `pir_status`, and `pir_notes`.
  - [x] `audit_logs`: Immutable tracking table storing action, actor employee ID, details, and timestamps.
  - [x] `users`: User account credentials with cryptographically secure salting, SHA-256 hashes, and privilege roles (`Admin`, `Manager`, `Agent`, `User`).
- [x] **Database Manager (`src/DatabaseManager.cs`)**:
  - [x] Connection pooling and automated DB creation (`bitsward_tickets`).
  - [x] Safe multi-statement script executor via `MySqlScript`.
  - [x] Parameterized query, scalar, and non-query helper routines.
- [x] **Organogram Synchronization (`src/OrganogramSync.cs`)**:
  - [x] Automated parsing of `org/organogram.csv`.
  - [x] **Pass 1**: Inserts/updates departments and base employee profiles.
  - [x] **Pass 2**: Resolves hierarchical `reports_to_id` links to establish direct supervisor reporting trees.
  - [x] Automatic execution on startup from `LoginForm.cs`.
- [x] **Automatic Database & Migration Checks**:
  - [x] Seamless schema loading from multi-depth output directories (`bin\Debug`).
  - [x] Backward-compatible dynamic column migration for `pir_status` and `pir_notes`.

---

### ✅ PHASE 2: Core Triage & SLA Backend Engines
- [x] **Ticket Classification Engine (`src/TriageEngine.cs`)**:
  - [x] Keyword analysis to classify tickets into **Incident (INC)**, **Service Request (SR)**, or **Change Request (CR)**.
  - [x] Department resolution routing (Cybersecurity/SOC, NOC/Network, Windows Admin, Linux Platform, Mac Admin, DevOps, QA, IT Systems).
- [x] **3-Tier Smart Assignment Routing Algorithm (`src/TriageEngine.cs`)**:
  - [x] **Tier 1 (Direct Supervisor)**: Assigns to creator's direct manager based on organogram tree if matching expertise.
  - [x] **Tier 2 (Workload Balancing)**: Identifies candidate engineers in the target department and assigns to the staff member with the lowest count of active (`Open`/`Assigned`/`In Progress`) tickets.
  - [x] **Tier 3 (Department Head / CTO Fallback)**: Assigns to Department Head or falls back to CTO (`MGT-001`) if all departmental staff are unavailable.
- [x] **Business-Hours SLA Resolution Engine (`src/SlaEngine.cs`)**:
  - [x] Computes realistic resolution deadlines strictly within corporate business hours (**09:00 AM – 05:00 PM, Monday to Friday**).
  - [x] Skips non-working weekend hours and outside business hours.
  - [x] Real-time breach detection (`IsBreached`).
  - [x] Warning threshold detection (`IsNearBreach`) based on SLA alert margins.

---

### ✅ PHASE 3: Modern UI & Presentation Layer
- [x] **Authentication Form (`src/LoginForm.cs`)**:
  - [x] Modern dark palette (`#1C2028`, `#252B36`, `#2980B9`).
  - [x] Masked password input with secure credential validation.
  - [x] Auto-seeds root administrator account (`admin` / `admin123`) linked to CTO identity if the system is empty.
  - [x] Automatic return-to-login on logout.
- [x] **Main Operational Dashboard (`src/MainForm.cs` & `MainForm.Designer.cs`)**:
  - [x] Top status bar with live logged-in user context and role badge.
  - [x] Left sidebar navigation with flat action buttons.
  - [x] 3-Tab triage queue separating **Incidents (INC)**, **Service Requests (SR)**, and **Change Requests (CR)**.
  - [x] Visual SLA color coding:
    - [x] Priority P1 cells formatted in high-visibility Red.
    - [x] Priority P2 cells formatted in Orange.
    - [x] SLA near-warning cells highlighted in soft Yellow.
    - [x] SLA breached cells highlighted in Red with dark red text.
  - [x] Split-container layout with responsive ticket details inspection panel.
  - [x] Defensive crash-prevention architecture:
    - [x] Case-insensitive null-safe column and cell access helpers (`FindColumn`, `FindCell`).
    - [x] `AutoSizeMode = None` layout-lock release before assigning explicit column widths.
    - [x] `BeginInvoke` deferred grid configuration on `DataBindingComplete`.
- [x] **Ticket Creation Dialog (`NewTicketDialog`)**:
  - [x] "Report Issue" action button in sidebar.
  - [x] Input prompts for Title, Priority (P1–P4), and Description.
  - [x] End-to-end integration with auto-triage classifier, target department resolver, SLA deadline calculator, and smart assignee router.

---

### ✅ PHASE 4: Functional Modules & Change Management
- [x] **Ticket Lifecycle Operations**:
  - [x] Supported status lifecycle: `Open` ➔ `Assigned` ➔ `In Progress` ➔ `Resolved` ➔ `Closed`.
  - [x] One-click **"Claim Ticket"** assigning ticket directly to logged-in user and advancing status to `Assigned`.
  - [x] Live status update dropdown with database sync.
- [x] **Agent Collision Avoidance (Soft Lock)**:
  - [x] Locks selected ticket for 2 minutes (`locked_by` and `locked_until` in DB).
  - [x] Heartbeat timer automatically refreshes active lock every 30 seconds.
  - [x] Shows visual yellow collision warning banner if another agent currently holds the lock.
  - [x] Automatically releases lock when deselecting, switching tabs, opening modals, or logging out.
- [x] **Thread History & Collaborative Actions**:
  - [x] Color-coded comment stream displaying timestamp, author, role, and message.
  - [x] Live thread note submission with instantaneous stream reload.
  - [x] **Sub-Task Splitting**: Allows agents to split thread items into linked sub-tasks stored in `tasks` table.
- [x] **Change Request (CR) Subsystem**:
  - [x] Dynamic CR control panel displayed exclusively when inspecting Change Request tickets.
  - [x] **Risk Profiler**: Interactive 3-question evaluation prompt (Environment Scope, Impact Level, Rollback Plan) calculating Low/Medium/High risk scores.
  - [x] **CAB Review**: Role-gated approval action (restricted to `Admin` and `Manager` accounts).
  - [x] **Maintenance Window Scheduler**: Start datetime and duration validation with conflict-safe formatting.
  - [x] **PIR & Rollback Engine**:
    - [x] `✓ PIR Success`: Documents post-implementation success criteria.
    - [x] `↩ PIR Rollback`: Mandatory rollback notes capture and status override.
    - [x] `📄 PIR Notes`: Quick-view modal dialog displaying recorded PIR notes and status.

---

### ✅ PHASE 5: Alerts, Reporting & Administration
- [x] **Task Manager (`src/TasksForm.cs` & `TasksForm.Designer.cs`)**:
  - [x] Accessible from sidebar ("📋 Tasks List").
  - [x] Supports global task inspection or ticket-specific filtering.
  - [x] Interactive status updater (`Pending`, `In Progress`, `Done`, `Cancelled`).
  - [x] Live task count indicator.
- [x] **Audit Log Viewer (`src/AuditLogForm.cs` & `AuditLogForm.Designer.cs`)**:
  - [x] Accessible from sidebar ("📜 Audit Logs").
  - [x] Displays complete event trail: LogID, TicketID, Employee Name, Role, Action, Details, and Timestamp.
  - [x] Supports ticket-filtered views and real-time refresh.
- [x] **Real-Time Notification System**:
  - [x] Background assignment monitor polling DB every 30 seconds.
  - [x] Custom non-blocking **Toast Notifications** (`ToastNotification`) rendered in bottom-right corner of screen.
  - [x] Uses Windows `WS_EX_NOACTIVATE` flag so popups never steal keyboard/focus during active work.
- [x] **Administration Console (`src/AdminForm.cs` & `AdminForm.Designer.cs`)**:
  - [x] Dual-tab interface: User Directory and Account Provisioning.
  - [x] Role modification (`Admin`, `Manager`, `Agent`, `User`).
  - [x] Password resetting with dynamic salt regeneration.
  - [x] Auto-populates unlinked employees from the organogram for rapid onboarding.

---

## ⏳ Future Roadmap & Extended Capabilities (Yet to be Implemented)

The following items are optional extended capabilities noted in `idea.txt` for future iterations:

| Feature | Category | Description | Priority |
| :--- | :--- | :--- | :---: |
| **File & Screenshot Attachments** | Enhancement | Add capability to upload/view log files and image attachments in ticket threads. | Medium |
| **Customer Self-Service Web Portal** | Web Layer | A lightweight browser-based portal for external non-IT staff to submit and track requests. | Medium |
| **SMTP / Email Notifications** | Notifications | Send external email alerts to clients/assignees on status changes and SLA warning events. | Low |
| **Interactive Floor & Room Maps** | UI Extension | Visual campus/floor-plan mapping for physical hardware issue location tagging. | Low |
| **PDF & Excel SLA Compliance Export** | Reporting | One-click export of audit logs, SLA breach statistics, and resolution time analytics. | Low |

---

## 📁 Repository Documentation Index

- [db_schema.txt](file:///d:/HOME_FILES/Documents/MD.%20Mosaddek-Al-Hameem/OOP/ticketingSystem/docs/db_schema.txt) — Database design & organogram sync documentation.
- [api_logic.txt](file:///d:/HOME_FILES/Documents/MD.%20Mosaddek-Al-Hameem/OOP/ticketingSystem/docs/api_logic.txt) — Triage engine, smart routing algorithms, and SLA logic.
- [auth_logic.txt](file:///d:/HOME_FILES/Documents/MD.%20Mosaddek-Al-Hameem/OOP/ticketingSystem/docs/auth_logic.txt) — Salting, password hashing, and role-based permissions.
- [ui_layout.txt](file:///d:/HOME_FILES/Documents/MD.%20Mosaddek-Al-Hameem/OOP/ticketingSystem/docs/ui_layout.txt) — UI color palette, form architecture, and component layout guide.
- [schema.sql](file:///d:/HOME_FILES/Documents/MD.%20Mosaddek-Al-Hameem/OOP/ticketingSystem/docs/schema.sql) — Production MySQL schema script with SLA and default seeds.
- [progress_log.txt](file:///d:/HOME_FILES/Documents/MD.%20Mosaddek-Al-Hameem/OOP/ticketingSystem/progress_log.txt) — Step-by-step state tracking log.
