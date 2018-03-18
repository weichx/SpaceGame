
A Definition of X is a type that describes how to create a single instance of that type.
It may "inherit" from a template of that same type. It will keep a link to it's template.
Definitions map 1-1 with their in-scene counterparts. A definition can be created fresh,
copied from another definition, or be stamped out from a template. A template is just a
definition that is not linked to a scene X. Definitions built from a template have a 
link to that template (by guid);

The reason to have these is that we want to be able to describe a thing's starting state
in the scene, definition and templates are never manipulated at runtime, only ready from.
