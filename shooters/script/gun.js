/**
 * Create new projectile
 * @param {Number} damage   - damage dealt in hp points
 * @param {Number} speed    - flying speed
 */
function Projectile(damage, speed) {
    this.damage = damage;
    this.speed = speed;
}

/**
 * Create new gun
 * @param {Projectile} projectile - type of projectile
 * @param {Number} distance   - fired as this maximum distance (in units)
 * @param {Number} rate       - rate of fire (shots per second)
 * @param {Number} clip       - clip size
 * @param {Number} reload     - reload time (milliseconds)
 */
function Gun(projectile, distance, rate, clip, reload) {
    this.projectile = projectile;
    this.distance = distance;
    this.rate = rate;
    this.clip = clip;
    this.reload = reload;
}