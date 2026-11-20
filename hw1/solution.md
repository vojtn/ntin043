
# Task 1 (Create models)

I created both diagrams in the UMLet web tool (UMLetino). It also supports a VS Code extension, which provides the option to edit or view diagrams directly from VS Code.

The main class diagram is stored in `class_diagram.png` and `class_diagram.uxf` (the UMLet source file).

The OCL constraints are included directly in the diagram as notes, and I also list them below in this Markdown file for better readability.

For the second diagram, I created an activity diagram showing the process of prescribing a new medicine. This is stored in `activity_diagram.uxf` and `activity_diagram.png`.

## OCL

1. Doctor must have an education specified
```ocl
context Doctor
inv: self.Education->size() > 0
```

2. Manufacture date must be before expiration date
```ocl
context MedicineItem
inv: self.DateOfManufacture < self.DateOfExpiration
```

3. Every room must have at least one shift assigned
```
context Room
inv: self.shift->size() > 0
```

4. Room occupation cannot exceed capacity (and cannot be negative)
```ocl
context Room
inv: self.current_occupation >= 0 and self.current_occupation <= self.capacity
```

5. Shift start time must be before end time
```ocl
context Shift
inv: self.from < self.to
```

6. Prescription ValidTo date has to be bigger than ValidFrom
```ocl
context Prescription
inv: self.ValidFrom < self.ValidTo
```

7. Total stock of medicine before collect has to be bigger than 0
```ocl
context Medicine
inv: self.medicineItem->collect(Amount)->sum() >= 1
```

8. A newly created room starts empty
```ocl
context Room::current_occupation : Int
init: 0
```

9. A newly registered medicine batch starts with 0 items until stocked
```ocl
context MedicineItem::Amount : Int
init: 0
```

10. Doctor examining a patient (produces a new MedicalRecord)
```ocl
context Doctor::Examine(p : Patient, d : MedicalDevice) : MedicalRecord
pre: self.Education->notEmpty() and self.shift->notEmpty()
post: p.medicalRecord->includes(result) and self.medicalRecord->includes(result)
```

11. Access control: Technician can only operate/service devices they oversee
```ocl
context Technician::OperateDevice(d : MedicalDevice) : Boolean
pre: self.medicalDevice->includes(d)
post: result = true
```

12. Derived attribute to detect if a room is full
```
context Room::isFull : Boolean 
derive: self.current_occupation >= self.capacity
```

# Task 2 (Document soution)

The hospital management system is divided into three main parts:
* **Staff & Scheduling:** `Employee` (inheriting from `Person`) is specialized into `Doctor`, `Nurse`, `Technician`, and `Cleaner` to clearly separate responsibilities and access rights. Employees are assigned to a `Department` and work scheduled `Shift`s taking place in specific `Room`s.

* **Inventory & Equipment:** Internal hospital assets inherit from `InventoryItem` (`MedicalDevice`, `OfficeDevice`, and `Furniture`), which tracks where items are located (`Room`) and which `Technician` oversees each `MedicalDevice`. This part is strictly internal and hidden from patients.

* **Patient Care & Medicine:** `Patient` (also inheriting from `Person`) is linked to their `MedicalRecord`s (written by a `Doctor`), `Prescription`s, and examinations. A ternary association connects `Doctor`, `Patient`, and `MedicalDevice` to capture which doctor examined which patient with which device. To track expiration dates and stock levels accurately, `Medicine` (drug definition) is separated from `MedicineItem` (physical batches stored in a `MedicineWarehouse`).

Key simplifying decisions include limiting each `Prescription` to a single `Medicine` and omitting overtime shifts. I think in real system used in Czechia the Presription (eRecept) can actually include more than one medicine.

The OCL constraints listed in Task 1 enforce data validity (invariants and initial values) as well as access control and operational rules (pre- and post-conditions for `Technician::OperateDevice` and `Doctor::Examine`).

For the second diagram, I created an activity diagram (`activity_diagram.png`) that models the workflow of a doctor prescribing medicine—covering access authorization, stock checking, refill requests, and updating the prescription.

# Task 3 (Discuss how to validate UML diagram in practise)
This question is about how to validate my diagram, my answer is that I would use some formal tools, which we used/will use in the practicals such as *Z, Allow or VDM* to formally specify and then validate the model.
Also I think the USE tool has some built in validation for OCL, so maybe If I wrote the project in it from begining, then it could be easy to validate there.

To be honest I'm not sure how differently to answer this question. Maybe it would also be a good idea that I would also used some LLM tool to help me to transform my UML and OCL to one of the mentioned formal languages, which that can be in my opinion pretty good helpful and save time in comparison to write it by hand.

Or actually I could also ask LLM to kinda validate or verify my diagrams to find some problems and issues.

# Task 4 (evaluate your solution)

- **Level of abstraction (what details were ignored):**  
  The model focuses on core internal hospital operations (staff, shifts, rooms, inventory, patient records, and prescriptions) and intentionally abstracts away details such as overtime shifts, billing/insurance, and external integrations. In a real hospital, the system would not live in isolation—it would integrate via APIs with external systems such as the **Ministry of Health** or **SÚKL** (e.g., for medicine registries and eRecept). Because I lack deep medical domain expertise, domain experts would likely identify additional hidden rules and relationships.

- **Level of approximation (over- vs. under-specification):**  
  - *Over-specification:* Modeling staff roles as separate subclasses inheriting from `Employee` (`Doctor`, `Nurse`, `Technician`, `Cleaner`) may be too rigid; a simpler `type` or `role` attribute on `Employee` could be sufficient and easier to extend. Similarly, limiting each `Prescription` to a single `Medicine` is more restrictive than the real Czech eRecept system, and requiring `self.shift->size() > 0` on every `Room` is too strict for rooms that do not have assigned shifts (e.g., storage rooms).
  - *Under-specification:* The model is still permissive in several areas—for example, it does not prevent an employee from being assigned overlapping shifts, does not enforce minimum night-shift staffing levels, and the ternary examination relation (`Doctor`–`Patient`–`MedicalDevice`) lacks timestamps to prevent double-booking a device.

- **Ambiguity versus precision:**  
  Adding explicit attribute types, multiplicities, and 12 OCL constraints (invariants, initial values, pre-/post-conditions, and a derivation rule) improved precision compared to an informal box-and-line sketch. However, some ambiguity remains, because I spent already a lot of time on the task: `Enum` values for `OfficeDevice` and `Furniture` are not explicitly listed.
  So Im aware of this issues, that it could be improved with more additional time.


# Task 5 (Prototype implementation)

In this task, I asked Gemini to generate a C# prototype skeleton based on the core classes and OCL constraints from my class diagram. The generated code is stored in `impl.cs`.

## Quality of the Generated Code
Overall, the AI did a good job. The generated C# code is clean, readable and uses modern C# features such as `DateOnly`, expression-bodied properties, LINQ (`.Sum()`), and appropriate exception types (e.g., `ArgumentException`, `ArgumentOutOfRangeException`, and `UnauthorizedAccessException` for access control).

## Relation Between the Code and the Specification
The mapping from UML and OCL to C# is mostly accurate:
- **Classes, inheritance, and attributes** match the conceptual diagram cleanly (`Person` → `Employee` → `Doctor`/`Technician`, `Patient`, `Room`, `Shift`, `Prescription`, `Medicine`, `MedicineItem`).
- **OCL invariants** are enforced using constructor checks and property setters (e.g., checking `from < to` in `Shift`, `ValidFrom < ValidTo` in `Prescription`, and `0 <= CurrentOccupation <= Capacity` in `Room`).
- **OCL initial values** (`init: 0`) map directly to C# field initializers (`= 0`).
- **OCL derivation rules** (`derive:`) map cleanly to a computed read-only property (`public bool IsFull => CurrentOccupation >= Capacity;`).
- **OCL pre- and post-conditions** are translated into runtime guard clauses at the start of methods (`Examine`, `OperateDevice`) and `Debug.Assert(...)` checks at the end, which in my opinion seems a bit odd

## Limitations and AI Mistakes
- **Ternary relation representation:** The ternary association (`Doctor`–`Patient`–`MedicalDevice`) is only loosely represented as method parameters in `Doctor.Examine(patient, device)` rather than as a dedicated link/entity. This is largely a limitation of standard object-oriented languages, which do not have native ternary associations and require an explicit helper class to store them.
- **Conflict between `init` and `inv`:** Because `MedicineItem.Amount` starts at `0` (`init: 0`) while `Medicine` requires total stock `>= 1` (`inv`), the AI could not enforce the stock invariant in the `Medicine` constructor and instead downgraded it to a `HasValidStock()` helper method. This actually might an issue with my written OCL requirements, that I didnt realized this problem.
- **Simplified scope:** To keep the prototype concise, some parts of the full diagram were omitted or flattened, which is okay, because it was specified in the prompt not to fully implement it.


# Task 6 (Opinion about the modeling language, tools, and approach)

## UML
I (like most other students) had already encountered UML in previous courses such as Database Systems (for relational modeling), Introduction to Software Engineering, and Data Formats. I also used a UML class diagram for the conceptual model in my bachelor's thesis, so the basic notation felt familiar.

Overall, I consider UML very useful—especially for conceptual modeling, because a well-structured class diagram is easy to understand even for non-IT stakeholders.

A newer aspect for me was behavioral modeling with activity and sequence diagrams. I chose an activity diagram for this homework, but I am not sure how heavily formal activity or sequence diagrams are used in everyday practice—often teams seem to rely on simpler flowcharts or use-case descriptions in other tools instead.

## Modeling Tools
I tried some of the tools shown in the practicals, such as USE and Papyrus UML, but both felt outdated and not very user-friendly. Their setup and installation were not straightforward, and I did not want to spend a lot of time just learning a clunky interface, so I stuck with **UMLetino**, which I already knew.

The downside of UMLetino is that it is not ideal for OCL. Writing OCL inside note boxes works for a few constraints, but on a larger diagram it quickly becomes cluttered and hurts readability and visibility—which is why I also listed the OCL constraints separately in Markdown.

## OCL
I had briefly seen OCL before in the Database Systems course, but I had not used it since. Writing operation pre-conditions, post-conditions, and derivation rules was mostly new to me.

In practice, I suspect OCL is not widely adopted in standard commercial projects. Companies often try to cut costs during the analysis and design phase, and writing thorough, syntactically correct OCL constraints takes significant time. Additionally, in larger projects, many OCL rules can quickly become hard to read and maintain.

## Formal vs. Informal UML in Practice
Based on my experience, sticking to **simpler informal diagrams** (boxes and lines) is usually enough for small-to-medium web/business apps where requirements change rapidly and full formal UML + OCL would create too much overhead. However, **proper formal UML with OCL** does make sense in specific scenarios:
- **Modeling the design of a complex or safety-critical system:** In domains like healthcare (hospital systems), aviation, or finance, catching unsafe states early (e.g., invalid prescriptions or double-booked devices) justifies the extra effort.
- **Capturing and validating requirements:** Formal multiplicities and OCL invariants force you to resolve ambiguities and check consistency before writing code.
- **Precise documentation:** When multiple teams or external contractors must integrate with a system, formal models serve as an unambiguous contract—something informal sketches cannot guarantee.


# Claim about AI tools usage 
I used AI tool (To be precise Google Gemini Pro) to help with the homework, but in limited way, I used it to check my UML diagrams and fix syntax of the OCL requirements. I didnt use it to generate whole entities or OCL itself. I also use it to discuss some stuff I wasnt sure from the task such as understanding ternary relation.

I also used it to check grammar and correct mistakes in this text, but the initial text was written by hand, AI was used only to correct it for better readability and not the text generation itself.

The only exception is the task 5 where the task itself required to try to generate some implementation of Class diagrams with LLM and compare it and discuss it.
