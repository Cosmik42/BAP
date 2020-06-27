# The Brick Automation Project

Welcome to "The Brick Automation Project" V1.5!

This software is free and developped for the community of AFOL!
I am extremely grateful to the following people for their kind donation.

Top Donnors:
 - David G.
 - Markus W.
 - Jacek H. 
 - Jade124
 - Martin H. 
 - Georg D.
 - Andrew H.
 - Isaac M.
 - Fabian W.
 
As of June 27th 2020, I have decided to share the source code of this project as I don't find time to work anymore on it.
I share it as-is, with its flaws :)

V1.5 is out -  08/21/19
----------------------------
- Control+ Hub - Implementation complete, including new L + XL Technic engines
- Fix a bug with EV3 preventing the use of more than one sensor at a time
- Fix a UI bug related to the Section part of the Self-Driving Module. That section now properly redraws after window resize.

V1.4 is out -  08/18/19
----------------------------
- The long awaited feature to limit connectivity to a list of devices during convention is done!
 * Choose between Project-Only limitation or Specific-List limitation.
- Fix ports of Boot Move Hub on latest firmware (Star Wars Boost Hub)

V1.3 is out -  05/04/19
----------------------------
- Update how motor are control to support latest PUP hardware.
- Alpha status because untested due to urgency of build an no availble hardware to test changes.

V1.2 is out -  03/16/19
----------------------------
- BuWizz - Implementation
- Play Sound implementation in the Event section


V1.1 is out -  02/19/19
----------------------------
- PFx - First implementation
- Improvement: Better Section Reservation for Self-Driving System.
- Bug Fix: Show proper battery level of remote controls
- Bug Fix: Motor Slider now properly show negative speed.
- Bug Fix: EV3 Motors properly activate on their ports.
- Bug Fix: Train not stopping immediately in Self-Driving module

V1.0.1 is out -  Hot Fix - 02/11/19
----------------------------
- Bug Fix: Remove hard-coded COM5 for EV3 connection and properly uses the COM input by the user
- Improvement: Stretch the length of the Hub names to allow Battery % to show up

V1.0 is officially released - 02/10/19
----------------------------
- New Name: The Brick Automation Project
- EV3 Support!
- Fix of SBrick port (port B and C were inverted)
- Add 'Released' event for PUP Remotes
- Fix Battery Level Not Showing up for PUP Hubs
- Introduce official terms of service

V0.9 is out - 02/03/19
----------------------------
- WeDo 2.0 Support
- Fix SBrick Sensor Calibration
- Fix Saving Issue with Remote Control


V0.8 is out - 02/02/19
----------------------------
- Remote Control Support
- LED Color Configuration
- Allow to start and stop scanning
- Allow to disconnect single devices

- Programming:
 * Support for Custom Trigger Events
 * Change the color of LEDs programmatically

- Self-Driving module: 
 * Implement Green/Red lights based on Section occupation.
 * Fix a bug with looped Paths

V0.7.1 is out - 01/28/19
----------------------------

- Programming:
 * Fix a bug preventing the access of custom code with events.

- Self-Driving module: 
 * Fix a bug preventing the deletion of Paths.
 * Fix a bug with 'Clear 2 Sections' in the Self-Driving Module 


V0.7 is out - 01/28/19
----------------------------

- Programming:
  * Plenty of new properties are now available
  * You can name sequences
  * You can customize distance/color trigger cooldown

 - Self-Driving module: 
  * You can now customize clearing time and 'stop needed ahead' speed
  * Add capacity to wait for the next 2 sections to clear
  * Allow to run code when next section releases
  * Speed Coefficient, to slow or accelerate trains based on battery level.


V0.6 is out - 01/20/19
----------------------------
 
- Self-Driving Trains!
- New UI
- More Robust Hub Detection
- Add Port Selection for Sensor Events
- Bug Fixes


V0.5 is out - 01/14/19
----------------------------

Major update includes:
- Full support of SBrick Plus and PF Sensors.
- Introducting an Anti-Collision system that works with simple distance detectors
- Bug fix: Code was executed twice on sensor event
- Bug fix: Better handling of faulty device connection

 
V0.4 is out - 01/14/19
----------------------------

Major update includes:
- SBrick support
- Global code editor

 
V0.3 is out - 12/26/18
----------------------------

Major update with this version:
- New UI
- Improved Hub Detection
- Color In C# Editor
- Light Management
- Code editor improvement
- Fix Hub not showing up on certain configurations
- Add 'State' variable to each hubs
 
Credits
-------

This software uses a couple other awesome open source code:
 - FastColoredTextBox: https://github.com/PavelTorgashov/FastColoredTextBox
 - Lego EV3 Library: https://github.com/BrianPeek/legoev3
