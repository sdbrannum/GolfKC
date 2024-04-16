import { ref } from "vue";

const KEY = "kcgolf-favorites";
const favorites = ref<string[]>([]);
const storageFavorites = window.localStorage.getItem(KEY);
if (storageFavorites) {
    favorites.value = JSON.parse(storageFavorites);
}
const getCourseKey = (source: string, courseId: string) =>
    `${source}:${courseId}`;

export function useFavorites() {
    const addFavorite = (source: string, courseId: string) => {
        favorites.value.push(getCourseKey(source, courseId));
        window.localStorage.setItem(KEY, JSON.stringify(favorites.value));
    };

    const removeFavorite = (source: string, courseId: string) => {
        const courseKey = getCourseKey(source, courseId);
        favorites.value = favorites.value.filter((v) => v !== courseKey);
        window.localStorage.setItem(KEY, JSON.stringify(favorites.value));
    };

    const isFavorite = (source: string, courseId: string) => {
        const courseKey = getCourseKey(source, courseId);
        return favorites.value.indexOf(courseKey) > -1;
    };

    return {
        addFavorite,
        removeFavorite,
        isFavorite,
    };
}
