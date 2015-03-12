# Why do I wish to override Pako? #

Pako was designed and created in 2008. We was a students, we learned a new languages and technologies and have no expirience in complex program design. The kerner and plug-ins is strongly integrated, some commands from plug-ins need some kernel code. That's all is bad. And code... It's difficult to read and understand.


# What I want to do? #

There is the next stages for Pako rewriting:
  * Rewrite kernel code. Write it as API interface.
  * Rewrite plugins to use the lernel API.

The kernel should have:
  * Connection API.
  * Security API.
  * Diagnostic and error-handling API.
  * Message sending API.
  * User description classes.
  * MUC description classes.
  * Plug-in handler with event-based plug-ins calling.
  * Memmory storage API.
  * Database access API to handle multi-database support (SQLite and MySQL for a first time).