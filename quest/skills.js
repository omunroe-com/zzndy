
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

    this.deception = 0;
    this.persuation = 0;
    this.barter = 0;

    this.traps = 0;    
    this.repair = 0;   // mechanics

    this.sneak = 0;
    this.steal = 0;
    this.lockpick = 0;
}

// Get how many skillpoints it costs to increase skill by one.
function skillPrice(current)
{
    if(current < 101)
        return 1;
    else if(current < 126)
        return 2;
    else if(current < 151)
        return 3;
    else if(current < 176)
        return 4;
    else if(current < 201)
        return 5;
    else if(current < 301)
        return 6;
    
    return Infinity;
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
