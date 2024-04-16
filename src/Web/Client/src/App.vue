<script setup lang="ts">
import { ref } from "vue";
import { getCourses } from "./api";
import CourseCard from "./components/CourseCard.vue";

const date = ref(null);
const today = new Date();
const offset = today.getTime() - today.getTimezoneOffset() * 60 * 1000;
const minDate = new Date(offset);
const minDateStr = minDate.toISOString().substring(0, 10);
const maxDate = new Date(offset);
maxDate.setDate(minDate.getDate() + 14);
const maxDateStr = maxDate.toISOString().substring(0, 10);

const courses = ref<Course[]>([]);

const loadCourses = async () => {
    const coursesResponse = await getCourses();
    courses.value = coursesResponse
        .sort((a, b) => a.name.localeCompare(b.name))
        .map((c) => ({
            ...c,
            teeTimes: [],
            loading: false,
        }));
};

loadCourses();
</script>

<template>
    <div class="h-full flex flex-col">
        <nav class="bg-slate-600 border-gray-200 dark:bg-gray-900 p-1">
            <input
                class="rounded mr-2 md:w-64 w-full"
                type="date"
                :min="minDateStr"
                :max="maxDateStr"
                v-model="date"
            />
        </nav>

        <main
            class="flex-1 flex flex-col items-center p-2"
            :class="{ 'justify-center': !!date }"
        >
            <template v-if="date">
                <course-card
                    v-for="course in courses"
                    :key="course.id"
                    v-bind="course"
                    :date="date"
                />
            </template>
            <template v-else>
                <p class="justify-self-center text-lg">Select a date</p>
            </template>
        </main>
    </div>
</template>

<style scoped>
.list-enter-active,
.list-leave-active {
    transition: all 0.5s ease;
}

.list-enter-from {
    opacity: 0;
    transform: translateX(30px);
}

.list-leave-to {
    opacity: 0;
    transform: translateX(-30px);
}

.list-leave,
.list-leave-active {
    position: absolute;
}
</style>
