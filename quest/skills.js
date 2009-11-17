
function Skills()
{
    this.firstAid = 0; // heal wounds
    this.medicine = 0; // heal cripled limbs
    this.science = 0;  // computers, advanced topics
    
    this.unarmed = 0;
    this.melee = 0;
    this.throwing = 0;

    this.pistols = 0;  // small guns
    this.rifles = 0;   // two-handed firearms
    this.bigGuns = 0;  // rocketlaunchers, miniguns and such
    this.energy = 0;   // lazer, plasma, pulse, gauss weapons.

    this.speech = 0;
    this.barter = 0;

    this.traps = 0;
    this.repair = 0;

    this.stealth = 0;
    this.steal = 0;
    this.lockpick = 0;
}

function Character(name, sex)
{
    this.name = name;
    this.sex = sex;

    this.skills = new Skills();
    this.xp = 0; // level might be a property being calculated based on XP
    
    /**
     * When character is using some skill it extents skillpoint alowance for that skill
     * which meens that upon levelup user will be able to spend more points on increasing
     * this skill.
     */
    this.alowances = new Skills();

    /**
     * When a book is read or any other kind of training paseed only small skillpoint
     * bonus is awarded but several next usages of the skill on which traing was focused 
     * yield double alowances.
     */
    this.trainingBonus = new Skills();
}
