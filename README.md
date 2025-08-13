# ia_salud_distraccion.py
"""
Chatbot de salud + menú de distracción.
Diseñado para ejecutarse en entornos con o sin Gradio/SSL.
Si Gradio no está disponible (por ejemplo, falta el módulo 'ssl'), el script hace un fallback a una interfaz de línea de comandos.
"""

import random
from dataclasses import dataclass
from typing import List, Tuple, Optional
import sys
import traceback

# Intento de import opcional de gradio
GRADIO_AVAILABLE = False
GRADIO_IMPORT_ERROR = None
try:
    import gradio as gr
    GRADIO_AVAILABLE = True
except Exception as e:
    GRADIO_AVAILABLE = False
    GRADIO_IMPORT_ERROR = e

DISCLAIMER = (
    "⚠️ Aviso: Esta información es educativa y no sustituye consejo médico profesional. "
    "Consulta a un especialista para diagnósticos o tratamientos."
)

TIPS_SALUD = [
    "Bebe 6–8 vasos de agua al día, ajusta si haces ejercicio o hace calor.",
    "Dormir 7–9 horas mejora el apetito, el ánimo y el rendimiento cognitivo.",
    "Realiza 150 min/sem de actividad moderada (o 75 min vigorosa) + fuerza 2 días.",
    "Llena medio plato con verduras y frutas en la mayoría de tus comidas.",
    "Prioriza alimentos mínimamente procesados: granos enteros, legumbres, frutas, verduras, frutos secos.",
    "Limita bebidas azucaradas; prefiere agua, infusiones o café sin azúcar.",
    "Toma pausas activas si estudias/trabajas sentado: 5 min de movimiento cada hora.",
    "Practica higiene del sueño: horario regular, menos pantallas 1h antes de dormir.",
]

DIETAS_BASE = {
    "Mediterránea": "Enfoque en frutas, verduras, legumbres, granos enteros, aceite de oliva, pescado; poca carne roja.",
    "Balanceada": "50% carbohidratos complejos, 25–30% proteínas, 20–25% grasas saludables; porciones moderadas.",
    "DASH": "Alta en potasio, calcio y magnesio; baja en sodio; ideal para presión arterial.",
    "Vegetariana": "Énfasis en legumbres, tofu/tempeh, lácteos/huevos opcionales, variedad de verduras y granos.",
}

CURIOSIDADES = [
    "Las zanahorias originalmente eran moradas, no naranjas.",
    "Caminar 10 minutos tras comer puede ayudar a controlar picos de glucosa.",
    "El corazón late ~100,000 veces al día en promedio.",
    "La avena contiene beta-glucanos, fibra asociada a mejor perfil lipídico.",
]

CHISTES = [
    "¿Qué hace una abeja en el gimnasio? ¡Zum-ba!",
    "—Doctor, me siento invisible. —¿Quién dijo eso?",
    "¿Qué le dice una impresora a otra? ¿Esa hoja es tuya o es una impresión mía?",
    "Ayer me caí en un círculo... ¡Menos mal que no fue en vano!",
]

TRIVIA = [
    ("¿Cuánta agua se recomienda en promedio al día para un adulto?",
     ["6–8 vasos", "1 vaso", "15 vasos"], 0),
    ("¿Cuántos minutos de actividad moderada a la semana sugiere la OMS?",
     ["150 minutos", "20 minutos", "300 minutos"], 0),
    ("¿Cuál es una grasa saludable típica de la dieta mediterránea?",
     ["Aceite de oliva", "Grasa trans", "Manteca hidrogenada"], 0),
]

OBJETIVOS = ["General", "Bajar grasa", "Ganar músculo", "Mejorar energía", "Control presión"]
TIPOS_DIETA = ["Omnívora", "Vegetariana", "Mediterránea", "DASH", "Balanceada"]

PLATOS = {
    "desayuno": {
        "Omnívora": [
            "Avena con plátano y maní + leche/alternativa",
            "Tostadas integrales con huevo revuelto y tomate",
            "Yogur natural con frutos rojos y granola",
        ],
        "Vegetariana": [
            "Avena con manzana y canela",
            "Tostadas integrales con hummus y pepino",
            "Yogur/soya con chía y mango",
        ],
        "Mediterránea": [
            "Pan integral con tomate y aceite de oliva + queso fresco",
            "Avena con frutos secos y miel",
            "Tortilla de espinaca con pan integral",
        ],
        "DASH": [
            "Avena con frutas y semillas, poca sal",
            "Pan integral con palta/aguacate y huevo",
            "Smoothie de plátano y espinaca (sin azúcar)",
        ],
        "Balanceada": [
            "Avena + fruta + nueces",
            "Huevos con verduras + pan integral",
            "Yogur + granola + fruta",
        ],
    },
    "almuerzo": {
        "Omnívora": [
            "Pollo a la plancha con quinoa y ensalada",
            "Arroz integral con atún y verduras salteadas",
            "Lomo saltado versión ligera con más verduras",
        ],
        "Vegetariana": [
            "Lentejas guisadas con arroz integral y ensalada",
            "Tacu tacu de frejoles con ensalada",
            "Tofu salteado con verduras y quinoa",
        ],
        "Mediterránea": [
            "Pescado al horno con papa y ensalada de tomate y olivas",
            "Ensalada grande con garbanzos, pepino, tomate y aceite de oliva",
            "Pasta integral con verduras, pesto y queso moderado",
        ],
        "DASH": [
            "Pechuga de pollo con camote y ensalada (bajo en sodio)",
            "Pescado a la plancha con arroz integral",
            "Ensalada de atún con verduras y legumbres",
        ],
        "Balanceada": [
            "Carne magra con arroz integral y ensalada",
            "Pasta integral con pollo y verduras",
            "Guiso de garbanzos con vegetales",
        ],
    },
    "cena": {
        "Omnívora": [
            "Ensalada de atún con palta y tomate",
            "Tortilla de verduras con pan integral",
            "Sopa de verduras con pollo desmenuzado",
        ],
        "Vegetariana": [
            "Revuelto de huevos con espinaca y champiñón",
            "Ensalada de quinoa con palta y tomate",
            "Crema de zapallo con semillas",
        ],
        "Mediterránea": [
            "Ensalada griega (tomate, pepino, aceitunas, queso) + pan integral",
            "Sopa minestrone ligera",
            "Pisto de verduras con huevo poché",
        ],
        "DASH": [
            "Sopa de verduras baja en sodio + pan integral",
            "Pavo al horno con ensalada",
            "Tortilla de claras con verduras",
        ],
        "Balanceada": [
            "Ensalada completa (proteína + carbo complejo + verduras)",
            "Sándwich integral de pavo/queso + ensalada",
            "Arroz integral con salteado de verduras y proteína",
        ],
    },
    "snack": [
        "Fruta + puñado de frutos secos",
        "Yogur natural",
        "Palitos de zanahoria con hummus",
        "Galletas de avena caseras",
        "Maíz cancha/pochoclo sin exceso de sal",
    ],
}

DIAS = ["Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo"]

@dataclass
class MenuSemanal:
    objetivo: str
    tipo: str
    calorias: int
    plan: List[Tuple[str, str, str, str, str]]


def generar_menu(objetivo: str, tipo: str, calorias: int = 2000) -> MenuSemanal:
    tipo_norm = tipo if tipo in PLATOS["desayuno"] else "Balanceada"
    plan = []
    random.seed(42 + len(tipo_norm) + len(objetivo))
    for d in DIAS:
        des = random.choice(PLATOS["desayuno"][tipo_norm])
        alm = random.choice(PLATOS["almuerzo"][tipo_norm])
        cen = random.choice(PLATOS["cena"][tipo_norm])
        snk = random.choice(PLATOS["snack"])
        plan.append((d, des, alm, cen, snk))
    return MenuSemanal(objetivo, tipo_norm, calorias, plan)


def formatear_menu(menu: MenuSemanal) -> str:
    header = f"Plan semanal ({menu.tipo}, objetivo: {menu.objetivo}, ~{menu.calorias} kcal)\n" \
             f"{DISCLAIMER}\n\n"
    filas = []
    for d, des, alm, cen, snk in menu.plan:
        filas.append(
            f"**{d}**\n- Desayuno: {des}\n- Almuerzo: {alm}\n- Cena: {cen}\n- Snack: {snk}\n"
        )
    return header + "\n".join(filas)


def clasificar_intencion(mensaje: Optional[str]) -> str:
    if not mensaje:
        return "general"
    m = mensaje.lower()
    if any(k in m for k in ["menu", "plan", "comer", "dieta"]):
        return "menu"
    if any(k in m for k in ["bajar", "grasa", "peso"]):
        return "bajar"
    if any(k in m for k in ["músculo", "musculo", "ganar fuerza", "proteína"]):
        return "musculo"
    if any(k in m for k in ["presión", "hipertensión", "sodio", "sal"]):
        return "presion"
    if any(k in m for k in ["agua", "dormir", "ejercicio", "hábitos", "habitos", "estrés", "estres"]):
        return "habitos"
    return "general"


def responder_salud(chat_hist, mensaje, objetivo, tipo_dieta, alergias):
    intent = clasificar_intencion(mensaje)
    base: List[str] = []
    if intent == "menu":
        base.append("Puedo generarte un menú semanal. Abre la pestaña 'Menú Semanal' o usa la opción correspondiente.")
    elif intent == "bajar":
        base += [
            "Déficit calórico moderado (10–20%) y 1.6–2.2 g proteína/kg de peso.",
            "Prioriza volumen de alimentos (verduras, frutas, legumbres) y bebidas sin azúcar.",
        ]
    elif intent == "musculo":
        base += [
            "Superávit ligero (5–10%), 1.6–2.2 g proteína/kg y entrenamiento de fuerza 2–4x/sem.",
            "Distribuye proteína en 3–5 comidas (20–40 g/ingesta).",
        ]
    elif intent == "presion":
        base += [
            "Enfoque DASH: más frutas/verduras, lácteos bajos en grasa, legumbres y menos sodio.",
            "Camina 30 min al día y limita ultraprocesados/embutidos.",
        ]
    elif intent == "habitos":
        base += [random.choice(TIPS_SALUD), random.choice(TIPS_SALUD)]
    else:
        base += [random.choice(TIPS_SALUD)]

    if alergias:
        base.append(f"⚠️ Considera tu(s) alergia(s): {alergias}. Lee etiquetas y evita trazas.")

    resp = "\n• ".join([DISCLAIMER] + base)
    chat_hist = (chat_hist or []) + [("Tú", mensaje or ""), ("IA", resp)]
    return chat_hist, chat_hist


OPCIONES_RPS = ["Piedra", "Papel", "Tijera"]


def jugar_rps(eleccion_usuario: str) -> str:
    if eleccion_usuario not in OPCIONES_RPS:
        return f"Opción inválida: {eleccion_usuario}. Elige entre {OPCIONES_RPS}."
    bot = random.choice(OPCIONES_RPS)
    if eleccion_usuario == bot:
        res = "Empate"
    elif (eleccion_usuario == "Piedra" and bot == "Tijera") or \
         (eleccion_usuario == "Papel" and bot == "Piedra") or \
         (eleccion_usuario == "Tijera" and bot == "Papel"):
        res = "¡Ganaste!"
    else:
        res = "Perdiste"
    return f"Tú: {eleccion_usuario} | IA: {bot} → {res}"


@dataclass
class EstadoNumero:
    objetivo: int = random.randint(1, 50)
    intentos: int = 0


def reiniciar_numero() -> EstadoNumero:
    return EstadoNumero()


def intentar_numero(estado: EstadoNumero, numero_usuario: int):
    try:
        numero_usuario = int(numero_usuario)
    except Exception:
        return estado, "Ingresa un número válido entre 1 y 50."
    estado.intentos += 1
    if numero_usuario == estado.objetivo:
        msg = f"¡Correcto! Era {estado.objetivo}. Intentos: {estado.intentos}. Reinicié el número."
        estado = reiniciar_numero()
    elif numero_usuario < estado.objetivo:
        msg = "Muy bajo. Intenta un número mayor."
    else:
        msg = "Muy alto. Intenta un número menor."
    return estado, msg


def pregunta_trivia(idx: int):
    q, opts, _ = TRIVIA[idx % len(TRIVIA)]
    return q, opts


def responder_trivia(idx: int, opcion: str) -> str:
    q, opts, ans = TRIVIA[idx % len(TRIVIA)]
    correcta = opts[ans]
    if opcion == correcta:
        return "✅ ¡Correcto!"
    return f"❌ Incorrecto. Respuesta: {correcta}"


# ---------------- Tests simples (no dep en SSL/Gradio) ----------------

def run_basic_tests() -> bool:
    ok = True
    try:
        m = generar_menu("General", "Mediterránea", 2000)
        assert isinstance(m, MenuSemanal) and len(m.plan) == 7
        s = formatear_menu(m)
        assert "Lunes" in s

        assert clasificar_intencion(None) == "general"
        assert clasificar_intencion("quiero un plan de dieta") == "menu"
        assert clasificar_intencion("quiero bajar de peso") == "bajar"

        r = jugar_rps("Piedra")
        assert "Tú:" in r and "IA:" in r

        estado = EstadoNumero(objetivo=10, intentos=0)
        estado, msg = intentar_numero(estado, 5)
        assert "Muy bajo" in msg
        estado, msg = intentar_numero(estado, 10)
        assert "Correcto" in msg

        correct_option = TRIVIA[0][1][TRIVIA[0][2]]
        assert responder_trivia(0, correct_option).startswith("✅")
        assert responder_trivia(0, "respuesta equivocada").startswith("❌")

    except AssertionError as e:
        print("[TEST FAILURE]:", e)
        traceback.print_exc()
        ok = False
    except Exception as e:
        print("[TEST ERROR]:", e)
        traceback.print_exc()
        ok = False

    print("Tests básicos:", "OK" if ok else "FALLÓ")
    return ok


# ---------------- Interfaz: Gradio (si está disponible) ----------------
if GRADIO_AVAILABLE:
    with gr.Blocks(title="IA Salud + Distracción", theme=gr.themes.Soft()) as demo:
        gr.Markdown("# 🤖 IA de Salud + 🎲 Menú de Distracción\n" + DISCLAIMER)

        with gr.Tab("Chat Salud/Dietas"):
            with gr.Row():
                chat = gr.Chatbot(height=350)
            with gr.Row():
                mensaje = gr.Textbox(label="Escribe tu pregunta (salud/dietas)")
            with gr.Row():
                objetivo = gr.Dropdown(choices=OBJETIVOS, value="General", label="Objetivo")
                tipo_dieta = gr.Dropdown(choices=TIPOS_DIETA, value="Balanceada", label="Tipo de dieta")
                alergias = gr.Textbox(label="Alergias/intolerancias (opcional)")
            enviar = gr.Button("Enviar")
            estado_chat = gr.State([])

            enviar.click(
                responder_salud,
                inputs=[estado_chat, mensaje, objetivo, tipo_dieta, alergias],
                outputs=[chat, estado_chat]
            )

        with gr.Tab("Menú Semanal"):
            gr.Markdown("Genera un menú de 7 días según tu objetivo y tipo de dieta.")
            objetivo_m = gr.Dropdown(choices=OBJETIVOS, value="General", label="Objetivo")
            tipo_m = gr.Dropdown(choices=TIPOS_DIETA, value="Balanceada", label="Tipo de dieta")
            calorias = gr.Slider(1200, 3200, value=2000, step=100, label="Calorías aproximadas")
            btn_menu = gr.Button("Generar menú")
            salida_menu = gr.Markdown()

            def _gen_menu(obj, tipo, kcal):
                m = generar_menu(obj, tipo, int(kcal))
                return formatear_menu(m)

            btn_menu.click(_gen_menu, inputs=[objetivo_m, tipo_m, calorias], outputs=[salida_menu])

        with gr.Tab("Distracción"):
            gr.Markdown("Tómate un break ✨")
            with gr.Row():
                with gr.Column():
                    gr.Markdown("### Chistes")
                    btn_chiste = gr.Button("Contar chiste")
                    out_chiste = gr.Textbox(label=" ", interactive=False)
                    btn_chiste.click(lambda: random.choice(CHISTES), outputs=out_chiste)

                with gr.Column():
                    gr.Markdown("### Curiosidades")
                    btn_cur = gr.Button("Dame una curiosidad")
                    out_cur = gr.Textbox(label=" ", interactive=False)
                    btn_cur.click(lambda: random.choice(CURIOSIDADES), outputs=out_cur)

            gr.Markdown("---")
            gr.Markdown("### Piedra, Papel o Tijera")
            elec = gr.Radio(OPCIONES_RPS, value="Piedra", label="Tu jugada")
            btn_rps = gr.Button("Jugar")
            out_rps = gr.Textbox(label="Resultado", interactive=False)
            btn_rps.click(jugar_rps, inputs=elec, outputs=out_rps)

            gr.Markdown("---")
            gr.Markdown("### Adivina el número (1–50)")
            estado_num = gr.State(EstadoNumero())
            numero = gr.Number(value=25, precision=0, label="Tu intento")
            btn_int = gr.Button("Probar")
            btn_res = gr.Button("Reiniciar juego")
            out_num = gr.Textbox(label="Pista", interactive=False)

            btn_int.click(intentar_numero, inputs=[estado_num, numero], outputs=[estado_num, out_num])
            btn_res.click(lambda: (EstadoNumero(), "¡Listo! Nuevo número secreto."), outputs=[estado_num, out_num])

        gr.Markdown("---\nHecho con ❤️. Recuerda: la salud es integral: alimentación, movimiento, sueño y emociones.")

# ---------------- Interfaz: CLI fallback ----------------

def cli_menu():
    print("IA Salud + Distracción (modo CLI)")
    estado_num = EstadoNumero()
    chat_hist = []
    while True:
        print("\n--- Menú principal ---")
        print("1) Chat salud/dietas")
        print("2) Generar menú semanal")
        print("3) Distracción")
        print("4) Tests básicos")
        print("5) Salir")
        choice = input("Elige una opción: ").strip()
        if choice == "1":
            msg = input("Escribe tu pregunta (salud/dietas): ")
            obj = input(f"Objetivo {OBJETIVOS} (enter para 'General'): ") or "General"
            tipo = input(f"Tipo de dieta {TIPOS_DIETA} (enter para 'Balanceada'): ") or "Balanceada"
            alerg = input("Alergias (opcional): ")
            chat_hist, _ = responder_salud(chat_hist, msg, obj, tipo, alerg)
            print("\n" + chat_hist[-1][1])
        elif choice == "2":
            obj = input(f"Objetivo {OBJETIVOS} (enter para 'General'): ") or "General"
            tipo = input(f"Tipo de dieta {TIPOS_DIETA} (enter para 'Balanceada'): ") or "Balanceada"
            kcal = input("Calorías aproximadas (enter para 2000): ") or "2000"
            menu = generar_menu(obj, tipo, int(kcal))
            print(formatear_menu(menu))
        elif choice == "3":
            while True:
                print("\n-- Distracción --")
                print("a) Chiste")
                print("b) Curiosidad")
                print("c) Piedra, Papel o Tijera")
                print("d) Adivina el número")
                print("e) Trivia")
                print("z) Volver")
                sub = input("Elige: ")
                if sub == "a":
                    print(random.choice(CHISTES))
                elif sub == "b":
                    print(random.choice(CURIOSIDADES))
                elif sub == "c":
                    jug = input(f"Tu jugada {OPCIONES_RPS}: ")
                    print(jugar_rps(jug))
                elif sub == "d":
                    try:
                        intento = int(input("Adivina (1-50): "))
                    except Exception:
                        print("Número inválido")
                        continue
                    estado_num, msg = intentar_numero(estado_num, intento)
                    print(msg)
                elif sub == "e":
                    for i, t in enumerate(TRIVIA):
                        print(i, t[0])
                    idx = int(input("Elige pregunta (número): "))
                    q, opts = pregunta_trivia(idx)
                    print(q)
                    for i, o in enumerate(opts):
                        print(i + 1, o)
                    sel = int(input("Elige opción (número): ")) - 1
                    print(responder_trivia(idx, opts[sel]))
                elif sub == "z":
                    break
                else:
                    print("Opción no válida")
        elif choice == "4":
            run_basic_tests()
        elif choice == "5":
            print("¡Adiós!")
            break
        else:
            print("Opción no válida.")


if __name__ == "__main__":
    if GRADIO_AVAILABLE:
        print("Gradio cargado correctamente. Ejecutando la interfaz web...")
        try:
            demo.launch()
        except Exception as e:
            print("Error al lanzar Gradio:", e)
            traceback.print_exc()
            print("Iniciando modo CLI como fallback...")
            cli_menu()
    else:
        print("Gradio no está disponible.")
        print("Import error:", repr(GRADIO_IMPORT_ERROR))
        print("Usando interfaz de línea de comandos (CLI). Si quieres la interfaz web, ejecuta este script en un entorno con 'gradio' y soporte SSL.")
        cli_menu()
