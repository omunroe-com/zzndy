// precedance
var prc = {'^':9, '×':8, '*':8, '÷':8, '/':8, '%':8, '+':6, '-':6, '(':-1};

/**
 * Tokenize expression.
 *
 * @param {String} expr  expression as a string
 * @return {Array}       array of tokens
 */
function tokenize(expr)
{
    function isEmpty(str) {
        return str.replace(/^\s+|\s+$/, '') != '';
    }

    function prettify(token)
    {
        switch (token) {
            case '*':
                return '×';
//            case '/':
//                return '÷';
            default:
                return token;
        }
    }

    return expr
            .replace(/([() +\/*^-])/g, '◊$1◊')
            .split('◊')
            .filter(isEmpty);
}

/**
 * Convert infix token queue to postfix token queue
 *
 * @param {Array} infix  queue of tokens from infix notation
 * @return {Array}       postfix queue
 */
function infix2postfix(infix)
{
    var out = [], opstack = [];

    for (var i = 0; i < infix.length; ++i)
    {
        var tok = infix[i];
        if (tok == ')')
        {
            while (opstack.length > 0 && opstack[opstack.length - 1] != '(')
                out.push(opstack.pop());

            if (opstack.length == 0) throw new Error('Unmatched paratheses');
            opstack.pop()
        }
        else if (tok == '(' || tok in prc) {
            while (tok != '(' && opstack.length > 0 && prc[tok] <= prc[opstack[opstack.length - 1]])
                out.push(opstack.pop());

            opstack.push(tok);
        }
        else
            out.push(tok);
    }

    var top;
    while (top = opstack.pop()) out.push(top);
    if (out[out.length - 1] == '(') throw new Error('Unmatched paratheses');

    return out;
}

/**
 * @constructor Expression
 * @param {Array} postfix    postfix expression queue
 * - or -
 * @param {String} operator  operator
 * @param {Object} left      left argument, Expression or value
 * @param {Object} right     right argument, Expression or value
 */
Expression = function() {
    switch (arguments.length) {
        case 1: // infix string
            var postfix = infix2postfix(tokenize(arguments[0]));
            var op = postfix.pop();
            var expr = getExpr(op, postfix);
            this.op = expr.op;
            this.left = expr.left;
            this.right = expr.right;
            break;
        case 3: // op, left, right
            this.op = arguments[0];
            this.left = arguments[1];
            this.right = arguments[2];
            break;
    }
    this.parent = null;
}

Expression.prototype.toString = function()
{
    var tx=[];
    if(this.parent && (this.op == '+' || this.op == '-')) tx.push('(');
    tx.push(this.left.toString());
    tx.push(' ');
    tx.push(this.op);
    tx.push(' ');
    tx.push(this.right.toString());
    if(this.parent && (this.op == '+' || this.op == '-')) tx.push(')');
    return tx.join('');
}

function getExpr(op, infix) {
    if (!(op in prc)) throw new Error('Expecting ' + op + ' to be an operator.');

    var expr = new Expression(op, null, null);
    var right = infix.pop();
    if (right in prc){
        expr.right = getExpr(right, infix);
        expr.right.parent = expr;
    }
    else expr.right = right;

    var left = infix.pop();
    if (left in prc){
        expr.left = getExpr(left, infix);
        expr.left.parent = expr;
    }
    else expr.left = left;

    return expr;
}

function expresseion2infix(expr)
{
    var infix = tokenize(expr);
    var postfix = infix2postfix(infix);
    var expression = new Expression(postfix);
    return expression;
}
