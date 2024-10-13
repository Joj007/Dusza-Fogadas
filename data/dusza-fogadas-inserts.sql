-- Create the database if it doesn't exist
CREATE DATABASE IF NOT EXISTS `dusza-fogadas` CHARACTER SET utf8 COLLATE utf8_hungarian_ci;

-- Use the newly created database
USE `dusza-fogadas`;

-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2024. Okt 13. 19:40
-- Kiszolgáló verziója: 10.4.32-MariaDB
-- PHP verzió: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `dusza-fogadas`
--

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `bets`
--

CREATE TABLE `bets` (
  `id` int(11) NOT NULL,
  `user_id` int(11) DEFAULT NULL,
  `game_id` int(11) DEFAULT NULL,
  `subject_id` int(11) DEFAULT NULL,
  `event_id` int(11) DEFAULT NULL,
  `bet_amount` int(11) NOT NULL,
  `bet_value` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `bets`
--

INSERT INTO `bets` (`id`, `user_id`, `game_id`, `subject_id`, `event_id`, `bet_amount`, `bet_value`) VALUES
(1, 1, 1, 1, 1, 20, '2'),
(2, 1, 1, 2, 1, 10, '1'),
(3, 2, 1, 1, 1, 10, '4'),
(4, 2, 1, 2, 1, 20, '5'),
(5, 2, 1, 2, 1, 10, '3');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `events`
--

CREATE TABLE `events` (
  `id` int(11) NOT NULL,
  `game_id` int(11) DEFAULT NULL,
  `description` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `events`
--

INSERT INTO `events` (`id`, `game_id`, `description`) VALUES
(1, 1, 'Hány gólt rugott?'),
(2, 2, 'programfutásának sebessége'),
(3, 2, 'programjának kimenete'),
(4, 2, 'programja hibát dob'),
(5, 3, 'Hány pontja lett?');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `games`
--

CREATE TABLE `games` (
  `id` int(11) NOT NULL,
  `organizer_id` int(11) DEFAULT NULL,
  `game_name` varchar(255) NOT NULL,
  `num_subjects` int(11) NOT NULL,
  `num_events` int(11) NOT NULL,
  `is_closed` tinyint(1) DEFAULT 0,
  `start_date` date NOT NULL,
  `close_date` date DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `games`
--

INSERT INTO `games` (`id`, `organizer_id`, `game_name`, `num_subjects`, `num_events`, `is_closed`, `start_date`, `close_date`) VALUES
(1, 3, 'Foci meccs', 2, 1, 1, '2024-10-13', '2024-10-13'),
(2, 3, 'Lajos és Bettina programjának futása', 2, 3, 0, '2024-10-13', NULL),
(3, 4, 'Matek verseny', 3, 1, 0, '2024-10-13', NULL);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `results`
--

CREATE TABLE `results` (
  `id` int(11) NOT NULL,
  `game_id` int(11) DEFAULT NULL,
  `subject_id` int(11) DEFAULT NULL,
  `event_id` int(11) DEFAULT NULL,
  `actual_value` varchar(255) NOT NULL,
  `multiplier` float(11,4) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `results`
--

INSERT INTO `results` (`id`, `game_id`, `subject_id`, `event_id`, `actual_value`, `multiplier`) VALUES
(1, 1, 1, 1, '2', 3.5000),
(2, 1, 2, 1, '4', 2.2500);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `subjects`
--

CREATE TABLE `subjects` (
  `id` int(11) NOT NULL,
  `game_id` int(11) DEFAULT NULL,
  `name` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `subjects`
--

INSERT INTO `subjects` (`id`, `game_id`, `name`) VALUES
(1, 1, 'Béla'),
(2, 1, 'Peti'),
(3, 2, 'Lajos'),
(4, 2, 'Bettina'),
(5, 3, 'Jani'),
(6, 3, 'Balázs'),
(7, 3, 'Kata');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `users`
--

CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL,
  `email` varchar(255) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `role` enum('admin','szervező','fogadó') NOT NULL,
  `balance` int(11) DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- A tábla adatainak kiíratása `users`
--

INSERT INTO `users` (`id`, `name`, `email`, `password_hash`, `role`, `balance`, `is_active`) VALUES
(1, 'fogado1', 'fogado1@gmail.com', 'c4318372f98f4c46ed3a32c16ee4d7a76c832886d887631c0294b3314f34edf1', 'fogadó', 140, 1),
(2, 'fogado2', 'fogado2@gmail.com', 'c4318372f98f4c46ed3a32c16ee4d7a76c832886d887631c0294b3314f34edf1', 'fogadó', 60, 1),
(3, 'szervezo1', 'szervezo1@gmail.com', '4ac58e2797f24a85c85b94e51299f7fa0851f1b1f02fcaaed3c75f297e3e5f19', 'szervező', NULL, 1),
(4, 'szervezo2', 'szervezo2@gmail.com', '4ac58e2797f24a85c85b94e51299f7fa0851f1b1f02fcaaed3c75f297e3e5f19', 'szervező', 100, 1),
(5, 'admin', 'admin@gmail.com', 'b2e277207179d53195ecf4bc79a3d3dfd5f55629e2ae1982784f35d8b868fc0d', 'admin', 100, 1),
(6, 'inaktiv', 'inaktiv@gmail.com', '331927bf1a855269df75774bc538e91029dc20e3849f15d1a3b51353a17ccb72', 'fogadó', 100, 0);

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `bets`
--
ALTER TABLE `bets`
  ADD PRIMARY KEY (`id`),
  ADD KEY `user_id` (`user_id`),
  ADD KEY `game_id` (`game_id`),
  ADD KEY `subject_id` (`subject_id`),
  ADD KEY `event_id` (`event_id`);

--
-- A tábla indexei `events`
--
ALTER TABLE `events`
  ADD PRIMARY KEY (`id`),
  ADD KEY `game_id` (`game_id`);

--
-- A tábla indexei `games`
--
ALTER TABLE `games`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `game_name` (`game_name`),
  ADD KEY `organizer_id` (`organizer_id`);

--
-- A tábla indexei `results`
--
ALTER TABLE `results`
  ADD PRIMARY KEY (`id`),
  ADD KEY `game_id` (`game_id`),
  ADD KEY `subject_id` (`subject_id`),
  ADD KEY `event_id` (`event_id`);

--
-- A tábla indexei `subjects`
--
ALTER TABLE `subjects`
  ADD PRIMARY KEY (`id`),
  ADD KEY `game_id` (`game_id`);

--
-- A tábla indexei `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `email` (`email`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `bets`
--
ALTER TABLE `bets`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT a táblához `events`
--
ALTER TABLE `events`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT a táblához `games`
--
ALTER TABLE `games`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT a táblához `results`
--
ALTER TABLE `results`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT a táblához `subjects`
--
ALTER TABLE `subjects`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT a táblához `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `bets`
--
ALTER TABLE `bets`
  ADD CONSTRAINT `bets_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`),
  ADD CONSTRAINT `bets_ibfk_2` FOREIGN KEY (`game_id`) REFERENCES `games` (`id`),
  ADD CONSTRAINT `bets_ibfk_3` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`),
  ADD CONSTRAINT `bets_ibfk_4` FOREIGN KEY (`event_id`) REFERENCES `events` (`id`);

--
-- Megkötések a táblához `events`
--
ALTER TABLE `events`
  ADD CONSTRAINT `events_ibfk_1` FOREIGN KEY (`game_id`) REFERENCES `games` (`id`);

--
-- Megkötések a táblához `games`
--
ALTER TABLE `games`
  ADD CONSTRAINT `games_ibfk_1` FOREIGN KEY (`organizer_id`) REFERENCES `users` (`id`);

--
-- Megkötések a táblához `results`
--
ALTER TABLE `results`
  ADD CONSTRAINT `results_ibfk_1` FOREIGN KEY (`game_id`) REFERENCES `games` (`id`),
  ADD CONSTRAINT `results_ibfk_2` FOREIGN KEY (`subject_id`) REFERENCES `subjects` (`id`),
  ADD CONSTRAINT `results_ibfk_3` FOREIGN KEY (`event_id`) REFERENCES `events` (`id`);

--
-- Megkötések a táblához `subjects`
--
ALTER TABLE `subjects`
  ADD CONSTRAINT `subjects_ibfk_1` FOREIGN KEY (`game_id`) REFERENCES `games` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
