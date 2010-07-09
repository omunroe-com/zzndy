
Deck = function(numberOfCards)
{
    this.deck = [];
    var n = 6;

    switch(numberOfCards)
    {
        case undefined:
        case 36:
            break;
        case 52:
            n=2;
            break;
        default:
            throw new Error('36 or 52 please');
    }

    var map = [];
    map[1] = 'A';
    map[11] = 'J';
    map[12] = 'Q';
    map[13] = 'K';

    for(var i=n;i<14;++i)
    {
        this.deck.push((i in map ? map[i] : i) + '\u2660');
        this.deck.push((i in map ? map[i] : i) + '\u2665');
        this.deck.push((i in map ? map[i] : i) + '\u2666');
        this.deck.push((i in map ? map[i] : i) + '\u2663');
    }

    this.deck.push('A\u2660');
    this.deck.push('A\u2665');
    this.deck.push('A\u2666');
    this.deck.push('A\u2663');

    this.deck.sort(function(){return Math.random() - .5});
    this.deck.sort(function(){return Math.random() - .5});
    this.deck.sort(function(){return Math.random() - .5});
    this.deck.sort(function(){return Math.random() - .5});
}

Deck.prototype.deal = function(n)
{
    return this.deck.splice(0, n);
}
