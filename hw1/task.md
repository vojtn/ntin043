topic: models in UML and OCL

domain: hospital management system (like for Motol or VFN)
	- some important entities to consider: personnel with different expertise (physicans, medics, surgeons, nurses, cleaners, technical support) and their shifts, basic equipment, advanced healthcare devices (X-ray, CT/MRI scanners, robotic surgery systems) and scheduling of their usage, database of patient records, medical drugs (inventory)
	- important aspect to consider: security (access control), avoiding unsafe situations (running out of drugs, too few staff on duty at night), explicit representation of real-world assumptions

task 1: create models for the specific domain
	- required: create an UML class diagram that captures relevant entities and relationships between them
		- define some binary and ternary relations between classes (with reasonable multiplicities)
	- required: create at least one UML diagram of some other type (preferably the sequence or activity diagram)
	- define some OCL constraints (derivation rules, invariants, pre-conditions and post-conditions)
	- models really should not be just drawings (boxes and lines), meaing that some level of precision and thorougness is required
		- what you need to specify: cardinalities for relations, some class members, OCL contraints, etc

task 2: document your solution
	- explain key design decisions (in plain text)

task 3: discuss how you would validate the UML diagrams in practice

task 4: evaluate your solution (model) according to the following criteria
	- level of abstraction (in the sense "what details you ignored")
	- level of approximation (over-, under-) in the sense "over-specification/over-constraining (specification is too restrictive) versus under-specification/under-constraining (model is too permissive)"
	- ambiguity versus precision
	- completeness (all scenarios)

task 5: create just a very basic skeleton of a prototype implementation of some part of the model (UML class diagram)
	- several classes and methods are enough here, preferably those annotated with OCL expressions in UML (it could be very small)
	- using modern LLM-based tools is allowed (recommended), such that input for LLM is the UML class diagram with OCL constraints
	- discuss the following: quality of the generated code, relation of code and specification, important and funny mistakes by AI

task 6: tell me your opinion about the modeling language, tools, and the whole approach (methodology) based on your personal experience
	- specifically, do you think it really makes sense to create proper UML diagrams with all the features in practice (and not sticking just to simpler informal diagrams)
	- possible usage scenarios to consider: modeling the design of a complex system, capturing and validating requirements (maybe for consistency), precise documentation

deadline: 20.11.2026

