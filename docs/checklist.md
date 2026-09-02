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
| **Phase 6: File & Screenshot Attachments** | ✅ Complete | AttachmentManager, AttachmentViewerForm, Clipboard Grab, NewTicket Attachments |
| **Phase 7: Manual Assignment & Smart Search** | ✅ Complete | AssigneeSearchDialog, Live Keystroke Filter, Workload Preview, NewTicketDialog Integration |
| **Future Extensions / Extended Roadmap** | ⏳ Planned | Web Customer Portal, SMTP Auto-Responder, PDF Export |

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
  - [x] Category / Type dropdown selection separating Incidents (INC), Service Requests (SR), and Change Requests (CR), with smart triage auto-detect fallback and dynamic UI header adaptation.
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
- [x] **Audit Log Viewer & Activity Engine (`src/AuditLogForm.cs` & `AuditLogForm.Designer.cs`)**:
  - [x] Accessible from sidebar ("📜 Audit Logs").
  - [x] Resilient logging architecture: relaxed constraints so logs persist even if initiated by administrator, unlinked accounts, or system routines.
  - [x] Automatic startup schema migration in `LoginForm.cs` removing restrictive foreign key constraint and setting `employee_id VARCHAR(100) NULL`, `ticket_id INT NULL`.
  - [x] Robust fallback hierarchy: resolves Employee Name, Username, or System context (`COALESCE(e.name, u.username, a.employee_id, 'System')`), preventing blank records.
  - [x] Full audit lifecycle coverage: ticket creation, status transitions, manual & smart assignments, comments, sub-task splitting/updates, risk evaluations, CAB reviews, and admin account provisioning/role modifications.
  - [x] Displays complete event trail: LogID, TicketID, Employee Name, Role, Action, Details, and Timestamp.
  - [x] Supports ticket-filtered views, live refresh, and direct Excel/CSV log exports.
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

### ✅ PHASE 6: File & Screenshot Attachments
- [x] **Database Schema (`ticket_attachments` table)**:
  - [x] Auto-incrementing ID, ticket FK, employee FK, file metadata (name, path, size, type), timestamp.
  - [x] `ON DELETE CASCADE` foreign keys for both `tickets` and `employees`.
  - [x] Safe auto-migration in `LoginForm.cs` on startup.
- [x] **Attachment Manager (`src/AttachmentManager.cs`)**:
  - [x] Physical file copy to `attachments/` directory with unique naming (`att_{ticketId}_{timestamp}_{guid}{ext}`).
  - [x] Clipboard image capture via `Clipboard.GetImage()` saved as lossless PNG screenshots.
  - [x] Attachment metadata CRUD: `GetAttachments()`, `GetAttachmentCount()`, `DeleteAttachment()`.
  - [x] File launching via `Process.Start()` and human-readable file size formatting.
- [x] **Attachment Viewer Form (`src/AttachmentViewerForm.cs` & `.Designer.cs`)**:
  - [x] Modal dialog listing all attachments in styled data grid with image preview panel.
  - [x] **Open**: Launch attachment in default system application.
  - [x] **Save As**: Export selected attachment to user-chosen location.
  - [x] **Add File**: Multi-select file browser for bulk uploads.
  - [x] **Paste Screenshot**: Clipboard image grab with user-friendly tips.
  - [x] **Delete**: Confirmation dialog with physical file + DB record cleanup.
  - [x] `AttachmentsChanged` event for real-time counter updates in MainForm.
- [x] **MainForm Integration**:
  - [x] `📎 Attachments (N)` counter button in ticket detail panel with dynamic highlight.
  - [x] Quick-attach `📎` button at bottom bar for inline file uploads with thread logging.
  - [x] Quick-screenshot `📸` button at bottom bar for one-click clipboard paste.
  - [x] Thread history entries auto-logged on each upload (`[📎 Attached file: ...]`, `[📸 Attached screenshot: ...]`).
- [x] **NewTicketDialog Attachment Support**:
  - [x] `📎 Attach File` button allowing multi-file selection before ticket submission.
  - [x] `📸 Paste Screenshot` button for clipboard capture during ticket creation.
  - [x] Live attachment counter summary label.
  - [x] Pending files/screenshots automatically saved after ticket ID is generated.

---

### ✅ PHASE 7: Manual Assignment & Smart User Search
- [x] **Smart Assignee Search Dialog (`src/AssigneeSearchDialog.cs` & `.Designer.cs`)**:
  - [x] Queries all created system users (`users` joined with `employees` and `departments`).
  - [x] Live keystroke filtering across Full Name, Username, Role, Department, Designation, and Employee ID.
  - [x] Displays real-time active ticket count (`ActiveTickets`) per user to visualize current workload.
  - [x] Keyboard friendly: Enter to confirm selection, Down Arrow to navigate grid, Esc to cancel.
  - [x] Dual action: `✓ Select Assignee` and `🔄 Auto-Assign (Triage)` reset option.
- [x] **NewTicketDialog UI Integration (`src/MainForm.cs`)**:
  - [x] `[👤 Assign...]` button opening smart search modal for all user roles (`Admin`, `Manager`, `Agent`, `User`).
  - [x] Smooth, uncluttered single-button layout aligning `[👤 Assign...]` with the assignee status label.
  - [x] Dynamic assignee status label (`Auto-Assign (Smart 3-Tier Routing)` or `👤 Name (@user - Role)` in green).
  - [x] `[✖]` reset button to quickly revert manual assignment back to 3-tier routing.
- [x] **Backend & Audit Trail Alignment (`src/MainForm.cs`)**:
  - [x] When manual assignee chosen: sets `assigned_employee_id`, updates status to `Assigned`, and records manual assignment in audit log.
  - [x] When unassigned: gracefully runs the smart 3-tier triage engine (Supervisor -> Workload Balancing -> Dept Head/CTO).
  - [x] Notification engine integration: newly assigned user receives non-blocking toast notification.

---

## ✅ Phase 8 — PDF & Excel SLA Compliance Export (Completed)

**Status:** ✅ Implemented | **Build:** 0 Warnings, 0 Errors

### Deliverables:
- [x] **SLA Report Analytics Engine (`src/SlaReportManager.cs`)**:
  - [x] Queries and aggregates ticket data with SLA deadlines via `SlaEngine.CalculateDeadline()`.
  - [x] Computes executive KPI metrics: Overall Compliance %, Avg Resolution Hours, Breach Count, Near-Breach Count.
  - [x] Priority-level breakdown (P1-P4) with per-level compliance percentages and average resolution times.
  - [x] Department-level compliance breakdown with ticket volumes and breach rates.
  - [x] Generates detailed SLA audit table with per-ticket compliance status (Compliant/Breached/At Risk/In Progress).
- [x] **Excel Export Engine (`src/ExcelReportExporter.cs`)**:
  - [x] Multi-worksheet XML Spreadsheet 2003 (`.xls`) with rich formatting: colored breach highlights, styled headers, KPI scorecards.
  - [x] Worksheet 1: Executive SLA Summary & KPIs + Priority + Department breakdown.
  - [x] Worksheet 2: Detailed Ticket SLA Audit with color-coded compliance status cells.
  - [x] Worksheet 3: Full Audit Trail Logs (bidirectionally joined, resolving Admin/Employee names and roles).
  - [x] Strict culture-invariant formatting (`CultureInfo.InvariantCulture`) preventing XML decimal corruption across localized operating systems.
  - [x] Null-safe cell handling eliminating cast exceptions on unassigned dates or durations.
  - [x] Clean, Excel-compliant worksheet identifiers and proper XML entity escaping.
  - [x] CSV (`.csv`) export for raw data ingestion.
  - [x] Zero external dependencies — pure C# XML generation.
- [x] **PDF Export Engine (`src/PdfReportExporter.cs`)**:
  - [x] Pure C# PDF 1.4 document generator — no third-party libraries.
  - [x] Dynamic multi-page table pagination: seamlessly renders large ticket volumes across Pages 2..N without truncation (up to 35 rows per page).
  - [x] Dynamic total page count calculation (`Page X of Y`).
  - [x] Strict PDF 1.4 specification compliance: exact 20-byte cross-reference (xref) offsets, font dictionaries with `/ProcSet [/PDF /Text]`, and stream byte synchronization.
  - [x] Executive styling: Page 1 dashboard with branded header, 5 KPI scorecard boxes, priority breakdown, and department breakdown.
  - [x] Pages 2..N: Table headers repeated on each page, alternating row fills, and color-coded SLA status badges (Green: Compliant, Red: Breached, Amber: At Risk, Blue: In Progress).
  - [x] Professional page footers with page numbering and confidentiality disclaimer.
- [x] **Interactive Reports Dashboard UI (`src/ReportsForm.cs` & `ReportsForm.Designer.cs`)**:
  - [x] Dark-themed analytics dashboard with filter bar (Date Range, Department, Priority).
  - [x] Live KPI scorecard cards: Compliance Rate, Total Volume, Avg Resolution Time, Breaches.
  - [x] Tabbed views: Priority & Department Breakdown | Detailed Ticket SLA Audit.
  - [x] Color-coded grid cells for compliance status visualization.
  - [x] Export action buttons: [📄 Export PDF] [📊 Export Excel] [📑 Export CSV] [Close].
- [x] **MainForm Sidebar Integration**: Added "📊 SLA Reports" navigation button opening `ReportsForm`.
- [x] **AuditLogForm Export Integration**: Added "📁 Export Logs" button for direct audit log CSV/Excel export.
- [x] **Project Registration**: All 5 new files registered in `BitswardITSM.csproj`.

---

## ⏳ Future Roadmap & Extended Capabilities (Yet to be Implemented)

The following items are optional extended capabilities noted in `idea.txt` for future iterations:

| Feature | Category | Description | Priority |
| :--- | :--- | :--- | :---: |
| **Customer Self-Service Web Portal** | Web Layer | A lightweight browser-based portal for external non-IT staff to submit and track requests. | Medium |
| **SMTP / Email Notifications** | Notifications | Send external email alerts to clients/assignees on status changes and SLA warning events. | Low |
| **Interactive Floor & Room Maps** | UI Extension | Visual campus/floor-plan mapping for physical hardware issue location tagging. | Low |

---

## 📁 Repository Documentation Index

- [db_schema.txt](file:///d:/HOME_FILES/Documents/MD.%20Mosaddek-Al-Hameem/OOP/ticketingSystem/docs/db_schema.txt) — Database design & organogram sync documentation.
- [api_logic.txt](file:///d:/HOME_FILES/Documents/MD.%20Mosaddek-Al-Hameem/OOP/ticketingSystem/docs/api_logic.txt) — Triage engine, smart routing algorithms, and SLA logic.
- [auth_logic.txt](file:///d:/HOME_FILES/Documents/MD.%20Mosaddek-Al-Hameem/OOP/ticketingSystem/docs/auth_logic.txt) — Salting, password hashing, and role-based permissions.
- [ui_layout.txt](file:///d:/HOME_FILES/Documents/MD.%20Mosaddek-Al-Hameem/OOP/ticketingSystem/docs/ui_layout.txt) — UI color palette, form architecture, and component layout guide.
- [schema.sql](file:///d:/HOME_FILES/Documents/MD.%20Mosaddek-Al-Hameem/OOP/ticketingSystem/docs/schema.sql) — Production MySQL schema script with SLA and default seeds.
- [progress_log.txt](file:///d:/HOME_FILES/Documents/MD.%20Mosaddek-Al-Hameem/OOP/ticketingSystem/progress_log.txt) — Step-by-step state tracking log.

