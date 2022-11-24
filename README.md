OBSOLETE (for 1.2 max)

BLOOD BANK: Transfusions for your colonists
===========================================
Are you sick of your pawns losing blood, and therefore productivity after defending your base? Introducing **Blood Bank**: a way to extract, store and transfuse blood between colonists, so you can keep on top of those pesky *Blood Loss (extreme)* effects and keep your pawns working!

### Description:
Adds a "Blood Pack" item that can be used to counter the effects of blood loss after injury. every organic creature (not just humanlikes) has an associated blood pack object (following the same rules for meat, so all bears have 'bear blood', etc).  After researching "Blood transfusions" (medieval tech, 500RP), Blood packs can be created using the "draw blood" operation and can be applied with the "give blood" operation by doctors with a medical skill of 4 or greater. Colonists are also able to use blood packs directly (like a serum) if their medical skill is high enough (useful in emergencies or when you only have one colonist left). 

Both of the operations and using a blood pack directly have 0 chance of failure. Blood has a short shelf-life so must be frozen if you plan on keeping a stockpile for any amount of time. Blood types are not important for the same reason that you can happily transplant organs without immunosuppresents in Rimworld - future human evolution/medicine/don't think about it :P You can find the 'Blood' category under 'Drugs' in stockpile filters.  Oh, and blood is drinkable and usable in cooking (dissallowed in all vanilla recipes by default). You sick bastards.

Extracting blood will reduce the donor's blood by 20%, and is considered a violation on prisoners, but not on downed allies, so you Warboys* will have to deal with negative faction relations and pawn thoughts (especially the one you stole blood from) if you try to make your own Tom Hardy. Taking blood from a pawn who already has extreme blood loss can kill them, so if you want to keep them alive only perform this operation on pawns with less than 80% blood loss.
	
Blood transfusions are not perfect and only restore half of the blood used to create a blood pack (10% blood loss returned), so you will need 2x as many blood packs to restore the same amount of blood taken. 

### Compatibility:
No idea if this steps on any toes compatibility-wise, but it isn't doing anything weird with vanilla logic and uses the Harmony library with postfixes for any code-injecty stuff so should be starting from a good place.  
Alien Framework compatiblity untested, human blood consuption thoughts probably fire for humanlike aliens who don't care about human meat - this'll get fixed.
	
### Ideas/Future plans:
- Refined Blood Products: refine part of your blood stockpile into useful compounds - healing boosters, immune boosters, performance boosters and field transfusion kits (with longer shelf-life than normal blood packs for battles far from home)
- Custom settings page for blood properties and operation effects (incl. failure below)
- Alien Framework Compatibility (alien blood != human blood for transfusions, extra blood products from exotic blood (maybe))
- Automatic job for doctors to apply emergency blood transfusions to patients with extreme blood loss if blood packs are available, like tending wounds.
- Failure chances:  a cut on the arm for a minor botched attempt, maybe up to poking out an eye for ridiculous, but I'm not too sold on making this dangerous at all
- Small chance of minor diesease: Transfusion Rejection, basically just food poisoning in effect, to represent increased (but not perfect) blood type compatibility in Rimworld's humans

	
### Note:
This is my first (public) mod. I searched for something similar when I was frustrated by pawns taking so long to heal back to a productive state after big fights and nothing came up, so here we are :) I hope you enjoy.


	
*Warboys and doofwarriors not included, but now I kinda want them... ahem watch this space.
