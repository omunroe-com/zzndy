/*

(2+2)*10-1 --->
    expr -
        left expr *
            left expr +
                left 2
                right 2
            right 10
        right 1

m2/s --->
    expr /
        left m2
        right s

infix to postfix

    * While there are tokens to be read:

        * Read a token.
        * If the token is a number, then add it to the output queue.
        * If the token is a function token, then push it onto the stack.
        * If the token is a function argument separator (e.g., a comma):

            * Until the topmost element of the stack is a left parenthesis, pop the element from the stack and push it onto the output queue. If no left parentheses are encountered, either the separator was misplaced or parentheses were mismatched.

        * If the token is an operator, o1, then:

            * while there is an operator, o2, at the top of the stack, and either

                    o1 is associative or left-associative and its precedence is less than (lower precedence) or equal to that of o2, or
                    o1 is right-associative and its precedence is less than (lower precedence) that of o2,

                pop o2 off the stack, onto the output queue;

            * push o1 onto the stack.

        * If the token is a left parenthesis, then push it onto the stack.
        * If the token is a right parenthesis:

            * Until the token at the top of the stack is a left parenthesis, pop operators off the stack onto the output queue.
            * Pop the left parenthesis from the stack, but not onto the output queue.
            * If the token at the top of the stack is a function token, pop it onto the output queue.
            * If the stack runs out without finding a left parenthesis, then there are mismatched parentheses.

    * When there are no more tokens to read:

        * While there are still operator tokens in the stack:

            * If the operator token on the top of the stack is a parenthesis, then there are mismatched parenthesis.
            * Pop the operator onto the output queue.

    * Exit.


*/

/**
 * Tokenize expression.
 *
 * @param {String} expr  expression as a string
 * @return {Array}       array of tokens
 */
function tokenize(expr)
{

}

/**
 * Convert infix token queue to postfix token queue
 *
 * @param {Array} infix  queue of tokens from infix notation
 * @return {Array}       postfix queue
 */
function infix2postfix(infix)
{

}

/**
 * @constructor Expression
 * @param {Array} postfix  postfix expression queue
 */
Expression = function(postfix){

}

function expresseion2infix(expr)
{
    var infix = tokenize(expr);
    var postfix = infix2postfix(infix);
    var expression = new Expression(postfix);
    return expression;
}