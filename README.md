# csModbus
## Full Featured C# Modbus Library

This library was created with the goal of clean architecture, especially with separate classes for the connection interface and the main class.
The slave (server site) contains a data server class that is intended to facilitate the access of the device application to the Modbus data.
An additional user control library supports the display of Modbus data in a desktop application.

Key Features:
* Master and Slave
* Interface TCP,UDP. RTU, ASCI
* Multimaster capable (TCP / UDP)
* Easy to use data server in slave mode
* Dataserver can manage multiple node-Ids
* additional gridview user control library 
* Demo Applications

Supported Modbus-Functions:
* READ_COILS = 0x01,
* READ_DISCRETE_INPUTS = 0x02,
* READ_HOLDING_REGISTERS = 0x03,
* READ_INPUT_REGISTERS = 0x04,
* WRITE_SINGLE_COIL = 0x05,
* WRITE_SINGLE_REGISTER = 0x06,
* WRITE_MULTIPLE_COILS = 0x0F,
* WRITE_MULTIPLE_REGISTERS = 0x10,
* READ_WRITE_MULTIPLE_REGISTERS = 0x17,     
