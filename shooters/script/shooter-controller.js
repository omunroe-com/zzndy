/**
 * Create new shooters controller
 * @param {ShooterView} view - shooters view handling display
 * @param {Array} shooters   - collection of shooters
 * @param {Number} width     - width in units
 * @param {Number} height    - height in units
 */
function ShooterController(view, shooters, width, height)
{
    this.view = view;
    this.shooters = shooters;
    this.width = width;
    this.height = height;
}