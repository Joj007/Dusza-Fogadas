# Használati útmutató

## Adatbázis

Az adatbázis működéséhez **XAMPP** szükséges. A XAMPP alkalmazásán belül el kell indítani az **Apache** és a **MySQL** modult. A MySQL sorában található *admin* gombbal megnyitható a **phpMyAdmin** felület.

1. A **phpMyAdmin** felületen az *Importálás* fülön kattints a *Fájl kiválasztása* gombra.
2. Tallózd ki a repository mappában lévő `data` mappa egyik adatbázisát:
   - A `dusza-fogadas.sql` fájl egy üres adatbázist importál.
   - A `dusza-fogadas-inserts.sql` fájl egy adatokat tartalmazó adatbázist importál.
3. A fájl kiválasztása után kattints az oldal alján található *Importálás* gombra. Ezzel az adatbázis létrejön.

## Program

A repository mappájában található `Dusza-Fogadas.sln` fájlt futtatva megnyílik a **Visual Studio**. Az alkalmazás futtatásához használd a **Ctrl + F5** billentyűkombinációt.

## Előre létrehozott fiókok belépési adatai:

### Fogadók:
- **Név:** fogado1 | **Jelszó:** Aa123456
- **Név:** fogado2 | **Jelszó:** Aa123456
- **Név:** inaktiv | **Jelszó:** Dd123456

### Szervezők:
- **Név:** szervezo1 | **Jelszó:** Bb123456
- **Név:** szervezo2 | **Jelszó:** Bb123456

### Admin:
- **Név:** admin | **Jelszó:** Cc123456

---

# Szerepkörök

## Fogadó:
- Saját összeg megtekintése
- Nyílt játékok böngészése
- Fogadás nyílt játékokra (1 alany-esemény párosra)

## Szervező:
- Játék létrehozása
- Játék lezárása, eredmények megadása

## Admin:
- Játékok törlése
- Felhasználók aktív állapotának módosítása
