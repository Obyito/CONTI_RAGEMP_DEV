$('#ConfirmRent1').click(() => {

    $('.alert').remove();
    mp.trigger('RentRollerToClient1');
});

$('#CloseButton').click(() => {

    $('.alert').remove();
    mp.trigger('RentVehicleClose');
});

$('#ConfirmRent2').click(() => {

    $('.alert').remove();
    mp.trigger('RentRollerToClient2');
});