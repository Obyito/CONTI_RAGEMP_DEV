let charBrowser = null;

mp.events.add("StartCharBrowser", () => {
	charBrowser = mp.browsers.new('package://Reallife/Character/Character.html');
	mp.gui.cursor.show(true, true);		
});
	
mp.events.add('CharacterInformationToServer', (vorname, nachname) => {
    mp.events.callRemote('OnPlayerCharacterAttempt', vorname, nachname);
});

mp.events.add('CharacterResult', (result) => {
    if (result == 1) {
		
        charBrowser.destroy();
        mp.gui.cursor.show(false, false);

        mp.gui.chat.push("Le nom / prenom a ete enregistre avec succes !");
    }
    else {

        mp.gui.chat.push('Mot de passe incorrect.');

        charBrowser.execute('document.getElementById("p1").innerHTML = "Donnees incorrectes";');
    }
});