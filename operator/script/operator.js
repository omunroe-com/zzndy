$(document).ready(function() {

    var placeholder = '<div class="placeholder">...</div>';

    $('.binary')
            .prepend(placeholder)
            .add('.unary')
            .append(placeholder)
            .draggable({revert:true, revertDuration: 150, helper: 'clone'});

    $('.query').droppable({drop:queryOnDrop, hoverClass: 'receiving', greedy: true});

    function queryOnDrop()
    {
        var helper = arguments[1].helper;
        $(this)
                .append(prepareHelper(helper))
                .find('.placeholder')
                .droppable({drop:placeholderOnDrop, hoverClass: 'receiving', greedy: true})
                .click(edit);
    }

    function placeholderOnDrop()
    {
        var helper = arguments[1].helper;
        $(this)
                .html(prepareHelper(helper))
                .prepend('<span class="op">(</span>')
                .append('<span class="op">)</span>')
                .droppable('destroy')
                .unbind('click', edit)
                .find('.placeholder')
                .droppable({drop:placeholderOnDrop, hoverClass: 'receiving', greedy: true})
                .click(edit);
    }

    function edit()
    {
        $(this)
                .droppable('destroy')
                .unbind('click', edit)
                .html('<input type="text" style="width: 3ex"/>')
                .find('input')
                .focus()
                .keypress(function(){console.log(this.value);$(this).css('width', (this.value.length + 3) + 'ex');});
    }

    function prepareHelper( helper )
    {
        return helper
                .clone()
                .removeClass('ui-draggable ui-draggable-dragging')
                .css({'position':'', 'top': '', 'left':''});
    }

});