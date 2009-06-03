$(document).ready(function() {

    var placeholder = '<div class="placeholder">...</div>';
    $('.binary').prepend(placeholder).add('.unary').append(placeholder).draggable({
        revert: true,
        revertDuration: 0,
        helper: 'clone'       ,
        activeClass: 'dragging'
    });

    $('.query').droppable({'tolerance': 'intersect',drop:function( event, object ) {
        console.log($(this)
                .append(prepareHelper(object.helper))
                .find('.placeholder'))
                .droppable({
            'tolerance':'intersect',
            'greedy':true,
            drop:function( event, object )
            {
                console.log(this);
                $(this).append(prepareHelper(object.helper));
            }
        })
                .end();
    }});


    function prepareHelper( helper )
    {
        return helper
                .clone()
                .removeClass('ui-draggable ui-draggable-dragging')
                .css({'position':'', 'top': '', 'left':''});
    }

});