--
-- PostgreSQL database dump
-- 4.4HD task: snake_case schema for Database-First scaffolding with Handlebars templates.
-- Tables `map` and `robot_command` and all of their columns use snake_case so that the
-- Handlebars templates can produce PascalCase C# property names automatically by
-- splitting on the underscore separator.
--

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Drop existing objects from the 4.3D schema so we can recreate them with snake_case names.
--

DROP TABLE IF EXISTS public.robotcommand CASCADE;
DROP TABLE IF EXISTS public.robot_command CASCADE;
DROP TABLE IF EXISTS public.map CASCADE;

--
-- Name: map; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.map (
    id integer NOT NULL,
    columns integer NOT NULL,
    rows integer NOT NULL,
    name character varying(50) NOT NULL,
    description character varying(800),
    created_date timestamp without time zone NOT NULL,
    modified_date timestamp without time zone NOT NULL,
    is_square boolean GENERATED ALWAYS AS (((rows > 0) AND (rows = columns))) STORED
);


ALTER TABLE public.map OWNER TO postgres;

--
-- Name: map_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.map ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.map_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


ALTER TABLE ONLY public.map
    ADD CONSTRAINT pk_map PRIMARY KEY (id);

--
-- Name: robot_command; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.robot_command (
    id integer NOT NULL,
    name character varying(50) NOT NULL,
    description character varying(800),
    is_move_command boolean NOT NULL,
    created_date timestamp without time zone NOT NULL,
    modified_date timestamp without time zone NOT NULL
);


ALTER TABLE public.robot_command OWNER TO postgres;

--
-- Name: robot_command_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.robot_command ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.robot_command_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


ALTER TABLE ONLY public.robot_command
    ADD CONSTRAINT pk_robot_command PRIMARY KEY (id);

--
-- Name: user; Type: TABLE; Schema: public; Owner: postgres
-- All columns use snake_case so the whole 4.4HD schema is consistent across every
-- table. UserDataAccess maps these columns manually (no scaffolding for this table).
--

DROP TABLE IF EXISTS public."user" CASCADE;

CREATE TABLE public."user" (
    id integer NOT NULL,
    email character varying(100) NOT NULL,
    first_name character varying(50) NOT NULL,
    last_name character varying(50) NOT NULL,
    password_hash character varying(500) NOT NULL,
    description character varying(800),
    role character varying(50),
    created_date timestamp without time zone NOT NULL,
    modified_date timestamp without time zone NOT NULL
);


ALTER TABLE public."user" OWNER TO postgres;

ALTER TABLE public."user" ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.user_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


ALTER TABLE ONLY public."user"
    ADD CONSTRAINT pk_user PRIMARY KEY (id);

ALTER TABLE ONLY public."user"
    ADD CONSTRAINT uq_user_email UNIQUE (email);

--
-- Data for Name: map; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.map (id, columns, rows, name, description, created_date, modified_date) FROM stdin;
1	5	5	DEAKIN	Default 5x5 square map	2022-07-30 00:00:00	2022-07-30 00:00:00
2	10	10	MOON	Large 10x10 square map	2022-07-30 00:00:00	2022-07-30 00:00:00
3	5	10	BURWOOD	Rectangular 5x10 map	2022-07-30 00:00:00	2022-07-30 00:00:00
\.


--
-- Data for Name: robot_command; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.robot_command (id, name, description, is_move_command, created_date, modified_date) FROM stdin;
1	LEFT	Turn the robot 90 degrees left	t	2022-07-30 00:00:00	2022-07-30 00:00:00
2	RIGHT	Turn the robot 90 degrees right	t	2022-07-30 00:00:00	2022-07-30 00:00:00
3	MOVE	Move the robot one step forward	t	2022-07-30 00:00:00	2022-07-30 00:00:00
4	PLACE	Place the robot at X,Y facing direction D	f	2022-07-30 00:00:00	2022-07-30 00:00:00
5	REPORT	Report the current position of the robot	f	2022-07-30 00:00:00	2022-07-30 00:00:00
\.


--
-- Data for Name: user; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."user" (id, email, first_name, last_name, password_hash, description, role, created_date, modified_date) FROM stdin;
1	admin@deakin.edu.au	Admin	User	AQAAAAIAAYagAAAAEAIcsGUs1W8GPnpnpuR7eBzAFVrE1OiExwfkB4VVlcDAf8oh4Gc7q+ThFCBFrATSpw==	Administrator account	Admin	2022-07-30 00:00:00	2022-07-30 00:00:00
2	user@deakin.edu.au	Updated	Name	AQAAAAIAAYagAAAAEBSWx5RM/VP1SVXWTSEc4wueJJxptjuE3AhGC3AMgq8dwLLkfJ4HDzvjJnLbcD+WAg==	Updated description	User	2022-07-30 00:00:00	2022-07-30 00:00:00
\.


--
-- Name: user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.user_id_seq', 2, true);


--
-- Name: map_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.map_id_seq', 4, true);


--
-- Name: robot_command_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.robot_command_id_seq', 6, true);


--
-- PostgreSQL database dump complete
--
