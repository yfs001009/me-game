from __future__ import annotations

import math
import random
from pathlib import Path

from PIL import Image, ImageDraw, ImageFilter, ImageFont


ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "Client.Unity" / "Assets" / "AssetRaw" / "UI" / "Art"


def font(size: int, bold: bool = False) -> ImageFont.FreeTypeFont:
    candidates = [
        "C:/Windows/Fonts/arialbd.ttf" if bold else "C:/Windows/Fonts/arial.ttf",
        "C:/Windows/Fonts/msyhbd.ttc" if bold else "C:/Windows/Fonts/msyh.ttc",
    ]
    for item in candidates:
        try:
            return ImageFont.truetype(item, size)
        except OSError:
            continue
    return ImageFont.load_default()


def lerp(a: int, b: int, t: float) -> int:
    return int(a + (b - a) * t)


def vertical_gradient(size: tuple[int, int], top: tuple[int, int, int], bottom: tuple[int, int, int]) -> Image.Image:
    w, h = size
    img = Image.new("RGB", size)
    px = img.load()
    for y in range(h):
        t = y / max(1, h - 1)
        color = tuple(lerp(top[i], bottom[i], t) for i in range(3))
        for x in range(w):
            px[x, y] = color
    return img.convert("RGBA")


def add_noise(img: Image.Image, amount: int = 10, alpha: int = 28) -> None:
    random.seed(11)
    w, h = img.size
    overlay = Image.new("RGBA", img.size, (0, 0, 0, 0))
    px = overlay.load()
    for y in range(0, h, 2):
        for x in range(0, w, 2):
            v = random.randint(-amount, amount)
            if v >= 0:
                px[x, y] = (255, 255, 255, min(alpha, v + alpha // 2))
            else:
                px[x, y] = (0, 0, 0, min(alpha, -v + alpha // 2))
    img.alpha_composite(overlay)


def rounded(draw: ImageDraw.ImageDraw, box, radius, fill, outline=None, width=1):
    draw.rounded_rectangle(box, radius=radius, fill=fill, outline=outline, width=width)


def draw_mountains(draw: ImageDraw.ImageDraw, w: int, h: int) -> None:
    ranges = [
        (0.48, (99, 126, 141), (218, 233, 232)),
        (0.56, (75, 104, 116), (204, 225, 224)),
        (0.66, (54, 86, 94), (185, 211, 208)),
    ]
    for y_factor, base, snow in ranges:
        base_y = int(h * y_factor)
        points = [(-80, base_y)]
        step = 210
        for x in range(-40, w + step, step):
            peak_y = base_y - random.randint(130, 280)
            points.append((x + random.randint(-60, 60), peak_y))
            points.append((x + step // 2, base_y + random.randint(-20, 40)))
        points.append((w + 80, h))
        points.append((-80, h))
        draw.polygon(points, fill=base)
        for x in range(0, w + step, step):
            peak = (x + random.randint(-60, 60), base_y - random.randint(130, 260))
            draw.polygon([peak, (peak[0] - 45, peak[1] + 78), (peak[0] + 38, peak[1] + 72)], fill=snow)


def draw_pine(draw: ImageDraw.ImageDraw, x: int, y: int, scale: float, snow: bool = True) -> None:
    trunk = (89, 61, 42)
    green = (36, 94, 65)
    dark = (24, 70, 54)
    draw.rectangle((x - int(8 * scale), y - int(45 * scale), x + int(8 * scale), y), fill=trunk)
    for i in range(4):
        yy = y - int((38 + i * 38) * scale)
        half = int((48 - i * 5) * scale)
        color = green if i % 2 == 0 else dark
        draw.polygon([(x, yy - int(60 * scale)), (x - half, yy + int(22 * scale)), (x + half, yy + int(22 * scale))], fill=color)
        if snow:
            draw.polygon([(x, yy - int(52 * scale)), (x - half // 2, yy), (x + half // 3, yy - int(2 * scale))], fill=(230, 244, 241))


def draw_cabin(draw: ImageDraw.ImageDraw, x: int, y: int, s: float) -> None:
    wall = (143, 86, 45)
    side = (111, 68, 42)
    roof = (101, 48, 35)
    roof_hi = (225, 238, 232)
    draw.rectangle((x, y - int(88 * s), x + int(150 * s), y), fill=wall)
    draw.rectangle((x + int(108 * s), y - int(88 * s), x + int(150 * s), y), fill=side)
    draw.polygon([(x - int(20 * s), y - int(86 * s)), (x + int(75 * s), y - int(160 * s)), (x + int(170 * s), y - int(86 * s))], fill=roof)
    draw.polygon([(x - int(10 * s), y - int(92 * s)), (x + int(75 * s), y - int(150 * s)), (x + int(160 * s), y - int(92 * s))], fill=roof_hi)
    draw.rectangle((x + int(56 * s), y - int(52 * s), x + int(93 * s), y), fill=(71, 46, 34))
    draw.rectangle((x + int(18 * s), y - int(68 * s), x + int(48 * s), y - int(38 * s)), fill=(255, 205, 92))
    draw.rectangle((x + int(110 * s), y - int(66 * s), x + int(138 * s), y - int(39 * s)), fill=(255, 205, 92))


def draw_login_bg() -> Image.Image:
    w, h = 1920, 1080
    img = vertical_gradient((w, h), (86, 151, 187), (238, 235, 216))
    d = ImageDraw.Draw(img)
    random.seed(3)
    draw_mountains(d, w, h)
    d.ellipse((1180, 92, 1420, 332), fill=(255, 219, 116, 150))
    d.rectangle((0, 690, w, h), fill=(221, 235, 226))
    for i in range(24):
        draw_pine(d, random.randint(-20, w), random.randint(700, 1040), random.uniform(0.65, 1.35))
    draw_cabin(d, 210, 815, 1.15)
    draw_cabin(d, 1390, 790, 0.95)
    d.rounded_rectangle((620, 156, 1300, 320), radius=38, fill=(68, 79, 78, 125), outline=(245, 230, 160, 160), width=6)
    d.text((960, 214), "SHEEP BATTLE", font=font(74, True), fill=(255, 239, 174), anchor="mm", stroke_width=5, stroke_fill=(83, 48, 33))
    add_noise(img, 8, 20)
    return img


def draw_lobby_bg() -> Image.Image:
    w, h = 1920, 1080
    img = vertical_gradient((w, h), (96, 157, 184), (223, 218, 179))
    d = ImageDraw.Draw(img)
    random.seed(8)
    draw_mountains(d, w, h)
    d.rectangle((0, 640, w, h), fill=(100, 151, 95))
    d.rectangle((0, 800, w, h), fill=(133, 116, 78))
    for i in range(38):
        draw_pine(d, random.randint(-20, w), random.randint(610, 860), random.uniform(0.45, 1.0), snow=False)
    # palisade wall
    for x in range(340, 1590, 42):
        d.rounded_rectangle((x, 690, x + 34, 910), radius=8, fill=(123, 78, 43), outline=(76, 49, 33), width=3)
        d.polygon([(x, 690), (x + 17, 650), (x + 34, 690)], fill=(94, 59, 38))
    d.rectangle((318, 760, 1610, 795), fill=(93, 61, 39))
    d.rectangle((318, 842, 1610, 878), fill=(93, 61, 39))
    # central hall
    draw_cabin(d, 815, 760, 1.85)
    d.rounded_rectangle((80, 74, 470, 178), radius=28, fill=(67, 89, 86, 175), outline=(248, 220, 130, 190), width=5)
    d.text((275, 124), "CAMP", font=font(58, True), fill=(255, 236, 170), anchor="mm", stroke_width=4, stroke_fill=(67, 43, 31))
    add_noise(img, 7, 18)
    return img


def panel(size: tuple[int, int], title: str | None = None) -> Image.Image:
    w, h = size
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    shadow = Image.new("RGBA", size, (0, 0, 0, 0))
    sd = ImageDraw.Draw(shadow)
    sd.rounded_rectangle((32, 38, w - 22, h - 18), radius=44, fill=(0, 0, 0, 95))
    shadow = shadow.filter(ImageFilter.GaussianBlur(10))
    img.alpha_composite(shadow)
    d.rounded_rectangle((24, 24, w - 36, h - 34), radius=44, fill=(99, 67, 43, 255), outline=(57, 36, 27, 255), width=8)
    d.rounded_rectangle((48, 54, w - 60, h - 66), radius=28, fill=(239, 219, 164, 255), outline=(150, 96, 54, 255), width=6)
    for x in range(70, w - 70, 68):
        d.ellipse((x - 10, 34, x + 10, 54), fill=(235, 196, 105), outline=(93, 58, 38), width=2)
    if title:
        d.rounded_rectangle((w * 0.27, 4, w * 0.73, 90), radius=28, fill=(139, 72, 48), outline=(70, 42, 32), width=5)
    add_noise(img, 6, 18)
    return img


def button(size: tuple[int, int], fill: tuple[int, int, int]) -> Image.Image:
    w, h = size
    img = Image.new("RGBA", size, (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    shadow = Image.new("RGBA", size, (0, 0, 0, 0))
    sd = ImageDraw.Draw(shadow)
    sd.rounded_rectangle((14, 18, w - 12, h - 8), radius=28, fill=(0, 0, 0, 100))
    shadow = shadow.filter(ImageFilter.GaussianBlur(6))
    img.alpha_composite(shadow)
    dark = tuple(max(0, c - 62) for c in fill)
    hi = tuple(min(255, c + 42) for c in fill)
    d.rounded_rectangle((10, 8, w - 14, h - 16), radius=28, fill=dark, outline=(70, 43, 31), width=5)
    d.rounded_rectangle((18, 10, w - 22, h - 26), radius=24, fill=fill)
    d.rounded_rectangle((24, 16, w - 28, h * 0.45), radius=20, fill=hi + (155,))
    add_noise(img, 5, 16)
    return img


def input_frame() -> Image.Image:
    w, h = 720, 126
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    d.rounded_rectangle((18, 22, w - 18, h - 14), radius=24, fill=(99, 64, 42), outline=(60, 38, 28), width=5)
    d.rounded_rectangle((36, 34, w - 36, h - 30), radius=18, fill=(255, 245, 207), outline=(177, 128, 72), width=4)
    d.rounded_rectangle((42, 40, w - 42, 62), radius=10, fill=(255, 255, 238, 120))
    add_noise(img, 4, 12)
    return img


def player_slot() -> Image.Image:
    w, h = 820, 150
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    d.rounded_rectangle((12, 16, w - 12, h - 14), radius=24, fill=(88, 62, 45), outline=(48, 32, 25), width=5)
    d.rounded_rectangle((32, 28, w - 32, h - 28), radius=18, fill=(225, 207, 157), outline=(148, 96, 56), width=4)
    d.ellipse((52, 42, 132, 122), fill=(92, 139, 154), outline=(57, 65, 68), width=4)
    d.ellipse((78, 58, 106, 88), fill=(255, 221, 163))
    d.rounded_rectangle((72, 88, 112, 120), radius=10, fill=(110, 74, 49))
    d.rounded_rectangle((158, 48, 480, 82), radius=10, fill=(128, 88, 52, 90))
    d.rounded_rectangle((158, 92, 350, 116), radius=8, fill=(128, 88, 52, 70))
    d.rounded_rectangle((626, 48, 760, 104), radius=16, fill=(78, 141, 91), outline=(50, 82, 55), width=3)
    add_noise(img, 5, 15)
    return img


def ribbon() -> Image.Image:
    w, h = 760, 190
    img = Image.new("RGBA", (w, h), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    d.polygon([(40, 42), (720, 42), (676, 98), (720, 154), (40, 154), (84, 98)], fill=(126, 61, 47), outline=(57, 34, 28))
    d.rounded_rectangle((92, 18, 668, 172), radius=34, fill=(155, 74, 51), outline=(67, 39, 31), width=6)
    d.rounded_rectangle((118, 42, 642, 132), radius=24, fill=(225, 181, 94), outline=(98, 62, 39), width=4)
    add_noise(img, 5, 14)
    return img


def save(img: Image.Image, name: str) -> None:
    OUT.mkdir(parents=True, exist_ok=True)
    img.save(OUT / name)


def main() -> None:
    save(draw_login_bg(), "login_bg_winter_camp.png")
    save(draw_lobby_bg(), "lobby_bg_fortified_camp.png")
    save(panel((1360, 860), "ROOM"), "panel_room_large.png")
    save(panel((940, 600), "NOTICE"), "panel_popup_common.png")
    save(button((460, 128), (60, 151, 83)), "button_primary_green.png")
    save(button((460, 128), (69, 129, 190)), "button_secondary_blue.png")
    save(button((460, 128), (157, 75, 58)), "button_danger_red.png")
    save(input_frame(), "input_frame_parchment.png")
    save(player_slot(), "room_player_slot_cartoon.png")
    save(ribbon(), "title_ribbon_sheep_battle.png")


if __name__ == "__main__":
    main()
